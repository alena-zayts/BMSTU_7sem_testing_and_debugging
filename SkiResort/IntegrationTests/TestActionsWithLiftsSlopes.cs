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
    [AllureSuite("SuiteForTestingActionsWithLiftsSlopes")]
    public class TestActionsWithLiftsSlopes : IDisposable
    {
        private TarantoolContext _tarantoolContext;
        private ILiftsRepository _liftsRepository;
        private ISlopesRepository _slopesRepository;
        private ILiftsSlopesRepository _liftsSlopesRepository;
        private IUsersRepository _usersRepository;
        private ICheckPermissionService _checkPermissionService;
        private LiftsSlopesService sut;

        public TestActionsWithLiftsSlopes()
        {
            _tarantoolContext = new TarantoolContext("ski_admin:Tty454r293300@localhost:3301");
            DBTablesCleaner dBTablesCleaner = new(_tarantoolContext);
            dBTablesCleaner.clean().GetAwaiter().GetResult();

            _liftsRepository = new TarantoolLiftsRepository(_tarantoolContext);
            _slopesRepository = new TarantoolSlopesRepository(_tarantoolContext);
            _liftsSlopesRepository = new TarantoolLiftsSlopesRepository(_tarantoolContext, _liftsRepository, _slopesRepository);
            _usersRepository = new TarantoolUsersRepository(_tarantoolContext);

            _checkPermissionService = new CheckPermissionsService(_usersRepository);
            sut = new LiftsSlopesService(_checkPermissionService, _liftsSlopesRepository, _liftsRepository, _slopesRepository);
        }
        public void Dispose()
        {
            DBTablesCleaner dBTablesCleaner = new(_tarantoolContext);
            dBTablesCleaner.clean().GetAwaiter().GetResult();
            _tarantoolContext.Dispose();
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestGetLiftsSlopesInfoAsync(List<LiftSlope> liftsSlopes)
        {
            // Arrange
            foreach (var liftSlope in liftsSlopes)
            {
                await _liftsSlopesRepository.AddLiftSlopeAsync(liftSlope.RecordID, liftSlope.LiftID, liftSlope.SlopeID);
            }
            var user = UsersObjectMother.AdminUser();
            await _usersRepository.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions);

            // act
            var liftsSlopesFromService = await sut.GetLiftsSlopesInfoAsync(user.UserID);

            //assert
            Assert.Equal(liftsSlopes.Count, liftsSlopesFromService.Count);
            foreach (var liftSlope in liftsSlopes)
                Assert.Contains(liftSlope, liftsSlopesFromService);
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAdminDeleteLiftSlopeAsync(Lift lift, Slope slope)
        {
            // Arrange
            await _liftsRepository.AddLiftAsync(lift.LiftID, lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime, lift.QueueTime);
            await _slopesRepository.AddSlopeAsync(slope.SlopeID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel);
            uint recordID = await _liftsSlopesRepository.AddLiftSlopeAutoIncrementAsync(lift.LiftID, slope.SlopeID);
            var user = UsersObjectMother.AdminUser();
            await _usersRepository.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions);
           
            // act
            await sut.AdminDeleteLiftSlopeAsync(user.UserID, lift.LiftName, slope.SlopeName);

            //assert
            Assert.Empty(await sut.GetLiftsSlopesInfoAsync(user.UserID));
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAdminAddAutoIncrementLiftSlopeAsync(Lift lift, Slope slope)
        {
            // Arrange
            await _liftsRepository.AddLiftAsync(lift.LiftID, lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime, lift.QueueTime);
            await _slopesRepository.AddSlopeAsync(slope.SlopeID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel);
            //uint recordID = await _liftsSlopesRepository.AddLiftSlopeAutoIncrementAsync(lift.LiftID, slope.SlopeID);
            var user = UsersObjectMother.AdminUser();
            await _usersRepository.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions);

            // act
            uint recordID = await sut.AdminAddAutoIncrementLiftSlopeAsync(user.UserID, lift.LiftName, slope.SlopeName);

            //assert
            LiftSlope liftSlopeFromService = (await sut.GetLiftsSlopesInfoAsync(user.UserID))[0];
            LiftSlope liftSlopeExpected = new LiftSlope(recordID, lift.LiftID, slope.SlopeID);
            Assert.Equal(liftSlopeExpected, liftSlopeFromService);
        }
    }
}
