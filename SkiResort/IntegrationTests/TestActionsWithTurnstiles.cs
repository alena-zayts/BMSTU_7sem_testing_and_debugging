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
    [AllureSuite("SuiteForTestingActionsWithTurnstiles")]
    public class TestActionsWithTurnstiles : IDisposable
    {
        private TarantoolContext _tarantoolContext;
        private IUsersRepository _usersRepository;
        private ITurnstilesRepository _turnstilesRepository;
        private ICheckPermissionService _checkPermissionService;
        private TurnstilesService sut;

        public TestActionsWithTurnstiles()
        {
            _tarantoolContext = new TarantoolContext("ski_admin:Tty454r293300@localhost:3301");
            DBTablesCleaner dBTablesCleaner = new(_tarantoolContext);
            dBTablesCleaner.clean().GetAwaiter().GetResult();

            _turnstilesRepository = new TarantoolTurnstilesRepository(_tarantoolContext);
            _usersRepository = new TarantoolUsersRepository(_tarantoolContext);
            _checkPermissionService = new CheckPermissionsService(_usersRepository);
            sut = new TurnstilesService(_checkPermissionService, _turnstilesRepository);
        }
        public void Dispose()
        {
            DBTablesCleaner dBTablesCleaner = new(_tarantoolContext);
            dBTablesCleaner.clean().GetAwaiter().GetResult();
            _tarantoolContext.Dispose();
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAdminGetTurnstileAsync(Turnstile turnstile)
        {
            // Arrange
            await _turnstilesRepository.AddTurnstileAsync(turnstile.TurnstileID, turnstile.LiftID, turnstile.IsOpen);
            var user = UsersObjectMother.AdminUser();
            await _usersRepository.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions);

            // act
            Turnstile turnstileFromService = await sut.AdminGetTurnstileAsync(user.UserID, turnstile.TurnstileID);

            //assert
            Assert.Equal(turnstile, turnstileFromService);
        }
        

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAdminGetTurnstilesAsync([Frozen] List<Turnstile> initialTurnstiles)
        {
            // Arrange
            foreach (var turnstile in initialTurnstiles)
            {
                await _turnstilesRepository.AddTurnstileAsync(turnstile.TurnstileID, turnstile.LiftID, turnstile.IsOpen);
            }
            var user = UsersObjectMother.AdminUser();
            await _usersRepository.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions);

            // act
            List<Turnstile> turnstilesFromService = await sut.AdminGetTurnstilesAsync(user.UserID);

            //assert
            Assert.Equal(initialTurnstiles.Count, turnstilesFromService.Count);
            foreach (var turnstile in turnstilesFromService)
            {
                Assert.Contains(turnstile, turnstilesFromService);
            }
        }


        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAdminUpdateTurnstileAsync(Turnstile turnstile)
        {
            // arrange
            var user = UsersObjectMother.AdminUser();
            await _usersRepository.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions);
            await _turnstilesRepository.AddTurnstileAsync(turnstile.TurnstileID, turnstile.LiftID, turnstile.IsOpen);

            // act
            await sut.AdminUpdateTurnstileAsync(user.UserID, turnstile.TurnstileID, turnstile.LiftID, !turnstile.IsOpen);

            //assert
            Assert.Equal(turnstile.IsOpen, ! (await sut.AdminGetTurnstileAsync(user.UserID, turnstile.TurnstileID)).IsOpen);
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAdminDeleteTurnstileAsync(Turnstile turnstile)
        {
            // arrange
            var user = UsersObjectMother.AdminUser();
            await _usersRepository.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions);
            await _turnstilesRepository.AddTurnstileAsync(turnstile.TurnstileID, turnstile.LiftID, turnstile.IsOpen);

            // act
            await sut.AdminDeleteTurnstileAsync(user.UserID, turnstile.TurnstileID);

            //assert
            Assert.Empty(await sut.AdminGetTurnstilesAsync(user.UserID));
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAdminAddAutoIncrementTurnstileAsync(Turnstile turnstile)
        {
            // arrange
            var user = UsersObjectMother.AdminUser();
            await _usersRepository.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions);

            // act
            uint turnstileIDFromService = await sut.AdminAddAutoIncrementTurnstileAsync(user.UserID, turnstile.LiftID, turnstile.IsOpen);

            //assert
            Assert.Equal(turnstile.LiftID, (await sut.AdminGetTurnstileAsync(user.UserID, turnstileIDFromService)).LiftID);
        }
    }
}