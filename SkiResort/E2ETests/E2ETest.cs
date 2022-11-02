using Xunit;
using BL.Services;
using BL.Models;
using BL.IRepositories;
using System.Collections.Generic;
using Allure.Xunit.Attributes;
//using IntegrationTests.ArrangeHelpers;
using System;
using AccessToDB;
using AccessToDB.RepositoriesTarantool;


//https://xunit.net/docs/shared-context
//Constructor and Dispose: when you want a clean test context for every test
// (sharing the setup and cleanup code, without sharing the object instance).


namespace E2ETests
{
    [AllureSuite("E2ETestSkiPatrolWorkingWithLifts")]
    public class E2ETest : IDisposable
    {
        private TarantoolContext _tarantoolContext;
        private ILiftsRepository _liftsRepository;
        private ISlopesRepository _slopesRepository;
        private ILiftsSlopesRepository _liftsSlopesRepository;
        private IUsersRepository _usersRepository;
        private ITurnstilesRepository _turnstilesRepository;
        private ICheckPermissionService _checkPermissionService;
        private UsersService _usersService;
        private LiftsService _liftsService;

        public E2ETest()
        {
            _tarantoolContext = new TarantoolContext("ski_admin:Tty454r293300@localhost:3301");
            DBTablesCleaner dBTablesCleaner = new(_tarantoolContext);
            dBTablesCleaner.clean().GetAwaiter().GetResult();

            _liftsRepository = new TarantoolLiftsRepository(_tarantoolContext);
            _slopesRepository = new TarantoolSlopesRepository(_tarantoolContext);
            _liftsSlopesRepository = new TarantoolLiftsSlopesRepository(_tarantoolContext, _liftsRepository, _slopesRepository);
            _usersRepository = new TarantoolUsersRepository(_tarantoolContext);
            _turnstilesRepository = new TarantoolTurnstilesRepository(_tarantoolContext);

            _checkPermissionService = new CheckPermissionsService(_usersRepository);
            _liftsService = new LiftsService(_checkPermissionService, _liftsRepository, _usersRepository, _liftsSlopesRepository, _turnstilesRepository);
            _usersService = new UsersService(_checkPermissionService, _usersRepository);
        }
        public void Dispose()
        {
            DBTablesCleaner dBTablesCleaner = new(_tarantoolContext);
            dBTablesCleaner.clean().GetAwaiter().GetResult();
            _tarantoolContext.Dispose();
        }

        [AllureXunitTheory]
        public async void E2ETestSkiPatrolWorkingWithLifts()
        {
            //arrange
            User adminUser = new User(100, 200, "admin_email", "admin_password", PermissionsEnum.ADMIN);
            await _usersRepository.AddUserAsync(adminUser.UserID, adminUser.CardID, adminUser.UserEmail, adminUser.Password, adminUser.Permissions);

            uint cardID = 10;
            string email = "user_email";
            string password = "user_password";


            Lift lift1 = new(new Lift(1, "A1", true, 10, 20, 30), new List<Slope>());
            Lift lift2 = new(new Lift(2, "A2", false, 40, 50, 60), new List<Slope>());
            Lift lift3 = new(new Lift(3, "B1", false, 40, 53, 62), new List<Slope>());
            List<Lift> lifts = new List<Lift>() { lift1, lift2, lift3 };
            foreach (Lift lift in lifts)
            {
                await _liftsRepository.AddLiftAsync(lift.LiftID, lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime, lift.QueueTime);
           }





            // New user registers (as usual)
            // act
            User workingUser = await _usersService.RegisterAsync(cardID, email, password);
            //assert
            Assert.Equal(password, workingUser.Password);
            Assert.Equal(email, workingUser.UserEmail);
            Assert.Equal(cardID, workingUser.CardID);
            Assert.Equal(PermissionsEnum.AUTHORIZED, workingUser.Permissions);


            // usual user looks at lifts
            // act
            List<Lift> liftsFromService = await _liftsService.GetLiftsInfoAsync(workingUser.UserID);
            //assert
            Assert.Equal(3, liftsFromService.Count);
            Assert.True(lifts[0].EqualWithoutConnectedSlopesAndQueueTime(liftsFromService[0]));
            Assert.True(lifts[1].EqualWithoutConnectedSlopesAndQueueTime(liftsFromService[1]));
            Assert.True(lifts[2].EqualWithoutConnectedSlopesAndQueueTime(liftsFromService[2]));


            // usual user tries to change one lift, but he has no rights
            // act & assert
            _ = Assert.ThrowsAsync<BL.Exceptions.PermissionExceptions.PermissionException>(
                async () => await _liftsService.UpdateLiftInfoAsync(workingUser.UserID, liftsFromService[0].LiftName, !liftsFromService[0].IsOpen, liftsFromService[0].SeatsAmount, liftsFromService[0].LiftingTime));


            // admin makes the user to be ski_patrol
            // act
            await _usersService.AdminUpdateUserAsync(adminUser.UserID, workingUser.UserID, workingUser.CardID, workingUser.UserEmail, workingUser.Password, PermissionsEnum.SKI_PATROL);
            // assert
            workingUser = await _usersService.AdminGetUserByIDAsync(adminUser.UserID, workingUser.UserID);
            Assert.Equal(password, workingUser.Password);
            Assert.Equal(email, workingUser.UserEmail);
            Assert.Equal(cardID, workingUser.CardID);
            Assert.Equal(PermissionsEnum.SKI_PATROL, workingUser.Permissions);


            // now ski_patrol user tries to change one lift, but he has no rights
            // act
            await _liftsService.UpdateLiftInfoAsync(workingUser.UserID, liftsFromService[0].LiftName, !liftsFromService[0].IsOpen, liftsFromService[0].SeatsAmount, liftsFromService[0].LiftingTime);
            // assert
            Lift updatedLift = await _liftsService.GetLiftInfoAsync(workingUser.UserID, liftsFromService[0].LiftName);
            Assert.Equal(liftsFromService[0].LiftID, updatedLift.LiftID);
            Assert.Equal(liftsFromService[0].LiftName, updatedLift.LiftName);
            Assert.Equal(!liftsFromService[0].IsOpen, updatedLift.IsOpen);
            Assert.Equal(liftsFromService[0].SeatsAmount, updatedLift.SeatsAmount);
            Assert.Equal(liftsFromService[0].LiftingTime, updatedLift.LiftingTime);
        }
    }
}