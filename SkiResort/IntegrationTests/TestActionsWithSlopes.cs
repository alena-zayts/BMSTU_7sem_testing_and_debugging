using Moq;
using Xunit;
using BL.Services;
using BL.Models;
using BL.IRepositories;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using Allure.Xunit.Attributes;
using IntegrationTests.ArrangeHelpers;
using System;
using AccessToDB;
using AccessToDB.RepositoriesTarantool;
using AccessToDB.Exceptions;


namespace IntegrationTests
{
    [AllureSuite("SuiteForTestingActionsWithSlopes")]
    public class TestActionsWithSlopes : IDisposable
    {
        private TarantoolContext _tarantoolContext;
        private ILiftsRepository _liftsRepository;
        private ISlopesRepository _slopesRepository;
        private ILiftsSlopesRepository _liftsSlopesRepository;
        private IUsersRepository _usersRepository;
        private ICheckPermissionService _checkPermissionService;
        private SlopesService sut;

        public TestActionsWithSlopes()
        {
            _tarantoolContext = new TarantoolContext("ski_admin:Tty454r293300@localhost:3301");
            DBTablesCleaner dBTablesCleaner = new(_tarantoolContext);
            dBTablesCleaner.clean().GetAwaiter().GetResult();

            _liftsRepository = new TarantoolLiftsRepository(_tarantoolContext);
            _slopesRepository = new TarantoolSlopesRepository(_tarantoolContext);
            _liftsSlopesRepository = new TarantoolLiftsSlopesRepository(_tarantoolContext, _liftsRepository, _slopesRepository);

            _usersRepository = new TarantoolUsersRepository(_tarantoolContext);
            _checkPermissionService = new CheckPermissionsService(_usersRepository);
            sut = new SlopesService(_checkPermissionService, _slopesRepository, _liftsSlopesRepository);
        }
        public void Dispose()
        {
            DBTablesCleaner dBTablesCleaner = new(_tarantoolContext);
            dBTablesCleaner.clean().GetAwaiter().GetResult();
            _tarantoolContext.Dispose();
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestGetSlopeInfoAsync(Slope slope)
        {
            // Arrange
            await _slopesRepository.AddSlopeAsync(slope.SlopeID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel);
            if (slope.ConnectedLifts != null)
            {
                foreach (Lift lift in slope.ConnectedLifts)
                {
                    await _liftsRepository.AddLiftAsync(lift.LiftID, lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime, lift.QueueTime);
                    await _liftsSlopesRepository.AddLiftSlopeAutoIncrementAsync(lift.LiftID, slope.SlopeID);
                }
            }
            var user = UsersObjectMother.AdminUser();
            await _usersRepository.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions);

            // act
            Slope slopeFromService = await sut.GetSlopeInfoAsync(user.UserID, slope.SlopeName);

            //assert
            Assert.True(slope.EqualWithoutConnectedLifts(slopeFromService));
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestGetSlopesInfoAsync([Frozen] List<Slope> initialSlopes)
        {
            // arrange
            var user = UsersObjectMother.AdminUser();
            await _usersRepository.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions);
            foreach (Slope slope in initialSlopes)
            {
                await _slopesRepository.AddSlopeAsync(slope.SlopeID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel);
            }
            List<Slope> slopes = new List<Slope> { };
            foreach (Slope slope in initialSlopes)
            {
                slopes.Add(new Slope(slope, new List<Lift> { }));
            }

            // act
            List<Slope> slopesFromService = await sut.GetSlopesInfoAsync(user.UserID);

            //assert
            Assert.Equal(slopes.Count, slopesFromService.Count);
        }


        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestUpdateSlopeInfoAsync(Slope slope)
        {
            // arrange
            await _slopesRepository.AddSlopeAsync(slope.SlopeID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel);
            var user = UsersObjectMother.AdminUser();
            await _usersRepository.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions);

            // act
            await sut.UpdateSlopeInfoAsync(user.UserID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel);

            //assert
            Assert.True(slope.EqualWithoutConnectedLifts(await sut.GetSlopeInfoAsync(user.UserID, slope.SlopeName)));
            
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAdminDeleteSlopeAsync(Slope slope)
        {
            // arrange
            await _slopesRepository.AddSlopeAsync(slope.SlopeID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel);
            var user = UsersObjectMother.AdminUser();
            await _usersRepository.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions);

            // act
            await sut.AdminDeleteSlopeAsync(user.UserID, slope.SlopeName);

            //assert
            Assert.Empty(await sut.GetSlopesInfoAsync(user.UserID));
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAdminAddAutoIncrementSlopeAsync(Slope slope)
        {
            // arrange
            var user = UsersObjectMother.AdminUser();
            await _usersRepository.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions);

            // act
            uint slopeIDFromService = await sut.AdminAddAutoIncrementSlopeAsync(user.UserID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel);

            //assert
            Slope slopeFromService = await sut.GetSlopeInfoAsync(user.UserID, slope.SlopeName);
            Assert.Equal(slopeFromService.SlopeID, slopeIDFromService);
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAdminAddSlopeAsync(Slope slope)
        {
            // arrange
            var user = UsersObjectMother.AdminUser();
            await _usersRepository.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions);


            // act
            await sut.AdminAddSlopeAsync(user.UserID, slope.SlopeID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel);

            //assert
            Assert.True(slope.EqualWithoutConnectedLifts(await sut.GetSlopeInfoAsync(user.UserID, slope.SlopeName)));
        }
    }
}