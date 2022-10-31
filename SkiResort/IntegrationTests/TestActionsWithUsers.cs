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
using BL.Exceptions.UserExceptions;


namespace IntegrationTests
{
    [AllureSuite("SuiteForTestingActionsWithSlopes")]
    public class TestActionsWithUsers : IDisposable
    {
        private TarantoolContext _tarantoolContext;
        private IUsersRepository _usersRepository;
        private ICheckPermissionService _checkPermissionService;
        private UsersService sut;
        private User adminUser;

        public TestActionsWithUsers()
        {
            _tarantoolContext = new TarantoolContext("ski_admin:Tty454r293300@localhost:3301");
            DBTablesCleaner dBTablesCleaner = new(_tarantoolContext);
            dBTablesCleaner.clean().GetAwaiter().GetResult();

            _usersRepository = new TarantoolUsersRepository(_tarantoolContext);
            _checkPermissionService = new CheckPermissionsService(_usersRepository);
            sut = new UsersService(_checkPermissionService,  _usersRepository);

            adminUser = new User(100, 200, "admin_email", "admin_password", PermissionsEnum.ADMIN);
            _usersRepository.AddUserAsync(adminUser.UserID, adminUser.CardID, adminUser.UserEmail, adminUser.Password, adminUser.Permissions);
        }
        public void Dispose()
        {
            DBTablesCleaner dBTablesCleaner = new(_tarantoolContext);
            dBTablesCleaner.clean().GetAwaiter().GetResult();
            _tarantoolContext.Dispose();
        }


        [AllureXunit]
        public async void TestRegisterOk()
        {
            // Arrange
            UserBuilder userBuilder = UserBuilder.aUser();
            User unauthorizedUser = userBuilder.inUnauthorizedRole().build();

            // act
            User resultUser = await sut.RegisterAsync(unauthorizedUser.CardID, unauthorizedUser.UserEmail, unauthorizedUser.Password);

            //assert
            Assert.Equal(unauthorizedUser.CardID, resultUser.CardID);
            Assert.Equal(unauthorizedUser.UserEmail, resultUser.UserEmail);
            Assert.Equal(unauthorizedUser.Password, resultUser.Password);
            Assert.Equal(PermissionsEnum.AUTHORIZED, resultUser.Permissions);
        }

        [AllureXunit]
        public async void TestRegisterUserWithNoEmail()
        {
            // Arrange
            User unauthorizedUser = UserBuilder.aUser().inUnauthorizedRole().withoutEmail().build();

            // act & assert
            _ = Assert.ThrowsAsync<UserRegistrationException>(async () => await sut.RegisterAsync(unauthorizedUser.CardID, unauthorizedUser.UserEmail, unauthorizedUser.Password));
        }

        [AllureXunit]
        public async void TestRegisterUserWithNoPassword()
        {
            // Arrange
            User unauthorizedUser = UserBuilder.aUser().inUnauthorizedRole().withoutPassword().build();
            
            // act & assert
            _ = Assert.ThrowsAsync<UserRegistrationException>(async () => await sut.RegisterAsync(unauthorizedUser.CardID, unauthorizedUser.UserEmail, unauthorizedUser.Password));
        }

        [AllureXunit]
        public async void TestRegisterUserWithRepeatedEmail()
        {
            // Arrange
            User unauthorizedUser = UserBuilder.aUser().inUnauthorizedRole().build();
            await _usersRepository.AddUserAsync(unauthorizedUser.UserID, unauthorizedUser.CardID, unauthorizedUser.UserEmail, unauthorizedUser.Password, unauthorizedUser.Permissions);
            
            // act & assert
            _ = Assert.ThrowsAsync<UserRegistrationException>(async () => await sut.RegisterAsync(unauthorizedUser.CardID, unauthorizedUser.UserEmail, unauthorizedUser.Password));
        }

        [AllureXunit]
        public async void TestLogInOk()
        {
            // Arrange
            UserBuilder userBuilder = UserBuilder.aUser();
            User unauthorizedUser = userBuilder.inUnauthorizedRole().build();
            User registeredUser = userBuilder.inAuthorizedRole().build();
            await _usersRepository.AddUserAsync(registeredUser.UserID, registeredUser.CardID, registeredUser.UserEmail, registeredUser.Password, registeredUser.Permissions);

            // act
            User resultUser = await sut.LogInAsync(unauthorizedUser.UserEmail, unauthorizedUser.Password);

            //assert
            Assert.Equal(registeredUser.CardID, resultUser.CardID);
            Assert.Equal(registeredUser.UserEmail, resultUser.UserEmail);
            Assert.Equal(registeredUser.Password, resultUser.Password);
            Assert.Equal(PermissionsEnum.AUTHORIZED, resultUser.Permissions);
        }

        [AllureXunit]
        public async void TestLogInWithWrongPassword()
        {
            // Arrange
            UserBuilder userBuilder = UserBuilder.aUser();
            User unauthorizedUser = userBuilder.inUnauthorizedRole().withPassword("321").build();
            User registeredUser = userBuilder.inAuthorizedRole().withPassword("123").build();
            await _usersRepository.AddUserAsync(registeredUser.UserID, registeredUser.CardID, registeredUser.UserEmail, registeredUser.Password, registeredUser.Permissions);

            // act & assert
            _ = Assert.ThrowsAsync<UserAuthorizationException>(async () => await sut.LogInAsync(unauthorizedUser.UserEmail, unauthorizedUser.Password));
        }

        [AllureXunit]
        public async void TestAdminGetUsers()
        {
            // Arrange
            List<User> usersList = new List<User> { UserBuilder.aUser().build() };
            foreach (User user in usersList)
            {
                await _usersRepository.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions);

            }

            // act
            List<User> resultUsers = await sut.AdminGetUsersAsync(adminUser.UserID);

            //assert
            usersList.Add(adminUser);
            Assert.Equal(usersList, resultUsers);
        }

        [AllureXunit]
        public async void TestAdminAddAutoIncrementUser()
        {
            // Arrange
            User newUser = UserBuilder.aUser().build();

            // act
            uint newUserID = await sut.AdminAddAutoIncrementUserAsync(adminUser.UserID, newUser.CardID, newUser.UserEmail, newUser.Password, newUser.Permissions);

            //assert
            User addedUser = await sut.AdminGetUserByIDAsync(adminUser.UserID, newUserID);
            Assert.Equal(newUser.CardID, addedUser.CardID);
            Assert.Equal(newUser.UserEmail, addedUser.UserEmail);
            Assert.Equal(newUser.Password, addedUser.Password);
            Assert.Equal(newUser.Permissions, addedUser.Permissions);
        }

        [AllureXunit]
        public async void TestAdminUpdateUser()
        {
            // Arrange
            User existingUser = UserBuilder.aUser().withPassword("123").build();
            string newPassword = "321";
            await _usersRepository.AddUserAsync(existingUser.UserID, existingUser.CardID, existingUser.UserEmail, existingUser.Password, existingUser.Permissions);

            // act
            await sut.AdminUpdateUserAsync(adminUser.UserID, existingUser.UserID, existingUser.CardID, existingUser.UserEmail, newPassword, existingUser.Permissions);

            //assert
            User userFromService = await sut.AdminGetUserByIDAsync(adminUser.UserID, existingUser.UserID);
            Assert.Equal(newPassword, userFromService.Password);
        }

        [AllureXunit]
        public async void TestAdminDeleteUser()
        {
            // Arrange
            User existingUser = UserBuilder.aUser().build();
            await _usersRepository.AddUserAsync(existingUser.UserID, existingUser.CardID, existingUser.UserEmail, existingUser.Password, existingUser.Permissions);
            uint userToDeleteID = existingUser.UserID;

            // act
            await sut.AdminDeleteUserAsync(adminUser.UserID, userToDeleteID);

            //assert
            Assert.Single((await sut.AdminGetUsersAsync(adminUser.UserID)));
            Assert.Equal(adminUser, (await sut.AdminGetUsersAsync(adminUser.UserID))[0]);
        }

        [AllureXunit]
        public async void TestAdminGetUserByIDAsync()
        {
            // Arrange
            User userToGet = UserBuilder.aUser().build();
            await _usersRepository.AddUserAsync(userToGet.UserID, userToGet.CardID, userToGet.UserEmail, userToGet.Password, userToGet.Permissions);
            

            // act
            User userFromService = await sut.AdminGetUserByIDAsync(adminUser.UserID, userToGet.UserID);

            //assert
            Assert.Equal(userToGet, userFromService);
        }
    }
}
