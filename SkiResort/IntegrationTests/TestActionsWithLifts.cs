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


//https://xunit.net/docs/shared-context
//Constructor and Dispose: when you want a clean test context for every test
// (sharing the setup and cleanup code, without sharing the object instance).


namespace IntegrationTests
{
    [AllureSuite("SuiteForTestingActionsWithLifts")]
    public class TestActionsWithLiftsLifts : IDisposable
    {
        private TarantoolContext _tarantoolContext;
        private ILiftsRepository _liftsRepository;
        private ISlopesRepository _slopesRepository;
        private ILiftsSlopesRepository _liftsSlopesRepository;
        private IUsersRepository _usersRepository;
        private ITurnstilesRepository _turnstilesRepository;
        private ICheckPermissionService _checkPermissionService;
        private LiftsService sut;

        public TestActionsWithLiftsLifts()
        {
            _tarantoolContext = new TarantoolContext("ski_admin:Tty454r293300@localhost:3301");
            DBTablesCleaner dBTablesCleaner = new(_tarantoolContext);
            dBTablesCleaner.clean().GetAwaiter().GetResult();

            _liftsRepository = new TarantoolLiftsRepository(_tarantoolContext);
            _slopesRepository = new TarantoolSlopesRepository(_tarantoolContext);
            _turnstilesRepository = new TarantoolTurnstilesRepository(_tarantoolContext);
            _liftsSlopesRepository = new TarantoolLiftsSlopesRepository(_tarantoolContext, _liftsRepository, _slopesRepository);
            _usersRepository = new TarantoolUsersRepository(_tarantoolContext);

            _checkPermissionService = new CheckPermissionsService(_usersRepository);
            sut = new LiftsService(_checkPermissionService, _liftsRepository, _usersRepository, _liftsSlopesRepository, _turnstilesRepository);
        }
        public void Dispose()
        {
            DBTablesCleaner dBTablesCleaner = new(_tarantoolContext);
            dBTablesCleaner.clean().GetAwaiter().GetResult();
            _tarantoolContext.Dispose();
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestGetLiftInfoAsync(Lift lift)
        {
            // Arrange
            await _liftsRepository.AddLiftAsync(lift.LiftID, lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime, lift.QueueTime);
            if (lift.ConnectedSlopes != null)
            {
                foreach (Slope slope in lift.ConnectedSlopes)
                {
                    await _slopesRepository.AddSlopeAsync(slope.SlopeID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel);
                    await _liftsSlopesRepository.AddLiftSlopeAutoIncrementAsync(lift.LiftID, slope.SlopeID);
                }
            }
            var user = UsersObjectMother.AdminUser();
            await _usersRepository.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions);

            // act
            Lift liftFromService = await sut.GetLiftInfoAsync(user.UserID, lift.LiftName);

            //assert
            Assert.True(lift.EqualWithoutConnectedSlopesAndQueueTime(liftFromService));
        }



        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestGetLiftsInfoAsync([Frozen] List<Lift> initialLifts)
        {
            // arrange
            var user = UsersObjectMother.AdminUser();
            await _usersRepository.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions);
            foreach (Lift lift in initialLifts)
            {
                await _liftsRepository.AddLiftAsync(lift.LiftID, lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime, lift.QueueTime);
            }
            List<Lift> lifts = new List<Lift> { };
            foreach (Lift lift in initialLifts)
            {
                lifts.Add(new Lift(lift, new List<Slope> { }));
            }

            // act
            List<Lift> liftsFromService = await sut.GetLiftsInfoAsync(user.UserID);

            //assert
            Assert.Equal(lifts.Count, liftsFromService.Count);
        }


        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestUpdateLiftInfoAsync(Lift lift)
        {
            // arrange
            await _liftsRepository.AddLiftAsync(lift.LiftID, lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime, lift.QueueTime);
            var user = UsersObjectMother.AdminUser();
            await _usersRepository.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions);

            // act
            await sut.UpdateLiftInfoAsync(user.UserID, lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime);

            //assert
            Assert.True(lift.EqualWithoutConnectedSlopesAndQueueTime(await sut.GetLiftInfoAsync(user.UserID, lift.LiftName)));

        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAdminDeleteLiftAsync(Lift lift)
        {
            // arrange
            await _liftsRepository.AddLiftAsync(lift.LiftID, lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime, lift.QueueTime);
            var user = UsersObjectMother.AdminUser();
            await _usersRepository.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions);

            // act
            await sut.AdminDeleteLiftAsync(user.UserID, lift.LiftName);

            //assert
            Assert.Empty(await sut.GetLiftsInfoAsync(user.UserID));
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAdminAddAutoIncrementLiftAsync(Lift lift)
        {
            // arrange
            var user = UsersObjectMother.AdminUser();
            await _usersRepository.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions);

            // act
            uint liftIDFromService = await sut.AdminAddAutoIncrementLiftAsync(user.UserID, lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime);

            //assert
            Lift liftFromService = await sut.GetLiftInfoAsync(user.UserID, lift.LiftName);
            Assert.Equal(liftFromService.LiftID, liftIDFromService);
        }
    }
}