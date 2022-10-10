using Moq;
using Xunit;
using BL.Services;
using BL.Models;
using BL.IRepositories;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using BL.Exceptions.UserExceptions;
using Allure.Xunit.Attributes;
using BL.Tests.ArrangeHelpers;



// лондонский вариант -- изоляция кода от зависимостей
// используется mock для: ICheckPermissionsService, (вызовы и взаимодействия, которые исполняются SUT к зависимым объектам)
// используется stub для: IUsersRepository (вызовы и взаимодействия,  которые исполняются SUT к зависимым объектам, чтобы запросить и получить  данные)
// не используется AutoMoqData, вместо него прямое создание Mock
// Используется паттерн DataBuilder для генерации объектов для тестов
namespace BL.Tests
{
    [AllureSuite("UsersServiceSuite")]
    public class UsersServiceTests
    {
        [AllureXunit]
        public async void TestRegisterOk()
        {
            // Arrange
            UserBuilder userBuilder = UserBuilder.aUser();
            User unauthorizedUser = userBuilder.inUnauthorizedRole().build();
            User authorizedUser = userBuilder.inAuthorizedRole().build();
            var usersRepositoryStub = new Mock<IUsersRepository>();
            {
                usersRepositoryStub.Setup(m => m.CheckUserEmailExistsAsync(unauthorizedUser.UserEmail)).ReturnsAsync(false);
                usersRepositoryStub.Setup(m => m.AddUserAutoIncrementAsync(unauthorizedUser.CardID, unauthorizedUser.UserEmail, unauthorizedUser.Password, authorizedUser.Permissions)).ReturnsAsync(unauthorizedUser.UserID);
                usersRepositoryStub.Setup(m => m.GetUserByEmailAsync(unauthorizedUser.UserEmail)).ReturnsAsync(authorizedUser);
            }
            var checkPermissionServiceMock = new Mock<ICheckPermissionService>();
            var sut = new UsersService(checkPermissionServiceMock.Object, usersRepositoryStub.Object);


            // act
            User resultUser = await sut.RegisterAsync(unauthorizedUser.CardID, unauthorizedUser.UserEmail, unauthorizedUser.Password);

            //assert
            usersRepositoryStub.Verify(m => m.CheckUserEmailExistsAsync(unauthorizedUser.UserEmail), Times.Once);
            usersRepositoryStub.Verify(m => m.AddUserAutoIncrementAsync(unauthorizedUser.CardID, unauthorizedUser.UserEmail, unauthorizedUser.Password, authorizedUser.Permissions), Times.Once);
            Assert.Equal(authorizedUser, resultUser);
        }

        [AllureXunit]
        public async void TestRegisterUserWithNoEmail()
        {
            // Arrange
            User unauthorizedUser = UserBuilder.aUser().inUnauthorizedRole().withoutEmail().build();
            var usersRepositoryStub = new Mock<IUsersRepository>();
            var checkPermissionServiceMock = new Mock<ICheckPermissionService>();
            var sut = new UsersService(checkPermissionServiceMock.Object, usersRepositoryStub.Object);

            // act & assert
            _ = Assert.ThrowsAsync<UserRegistrationException>(async () => await sut.RegisterAsync(unauthorizedUser.CardID, unauthorizedUser.UserEmail, unauthorizedUser.Password));
        }

        [AllureXunit]
        public async void TestRegisterUserWithNoPassword()
        {
            // Arrange
            User unauthorizedUser = UserBuilder.aUser().inUnauthorizedRole().withoutPassword().build();
            var usersRepositoryStub = new Mock<IUsersRepository>();
            var checkPermissionServiceMock = new Mock<ICheckPermissionService>();
            var sut = new UsersService(checkPermissionServiceMock.Object, usersRepositoryStub.Object);

            // act & assert
            _ = Assert.ThrowsAsync<UserRegistrationException>(async () => await sut.RegisterAsync(unauthorizedUser.CardID, unauthorizedUser.UserEmail, unauthorizedUser.Password));
        }

        [AllureXunit]
        public async void TestRegisterUserWithRepeatedEmail()
        {
            // Arrange
            User unauthorizedUser = UserBuilder.aUser().inUnauthorizedRole().build();
            var usersRepositoryStub = new Mock<IUsersRepository>();
            {
                usersRepositoryStub.Setup(m => m.CheckUserEmailExistsAsync(unauthorizedUser.UserEmail)).ReturnsAsync(true);
            }
            var checkPermissionServiceMock = new Mock<ICheckPermissionService>();
            var sut = new UsersService(checkPermissionServiceMock.Object, usersRepositoryStub.Object);

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
            var usersRepositoryStub = new Mock<IUsersRepository>();
            {
                usersRepositoryStub.Setup(m => m.GetUserByEmailAsync(unauthorizedUser.UserEmail)).ReturnsAsync(registeredUser);
            }
            var checkPermissionServiceMock = new Mock<ICheckPermissionService>();
            var sut = new UsersService(checkPermissionServiceMock.Object, usersRepositoryStub.Object);


            // act
            User resultUser = await sut.LogInAsync(unauthorizedUser.UserEmail, unauthorizedUser.Password);

            //assert
            usersRepositoryStub.Verify(m => m.GetUserByEmailAsync(unauthorizedUser.UserEmail), Times.Once);
            Assert.Equal(registeredUser, resultUser);
        }

        [AllureXunit]
        public async void TestLogInWithWrongPassword()
        {
            // Arrange
            UserBuilder userBuilder = UserBuilder.aUser();
            User unauthorizedUser = userBuilder.inUnauthorizedRole().withPassword("321").build();
            User registeredUser = userBuilder.inAuthorizedRole().withPassword("123").build();
            var usersRepositoryStub = new Mock<IUsersRepository>();
            {
                usersRepositoryStub.Setup(m => m.GetUserByEmailAsync(unauthorizedUser.UserEmail)).ReturnsAsync(registeredUser);
            }
            var checkPermissionServiceMock = new Mock<ICheckPermissionService>();
            var sut = new UsersService(checkPermissionServiceMock.Object, usersRepositoryStub.Object);

            // act & assert
            _ = Assert.ThrowsAsync<UserAuthorizationException>(async () => await sut.LogInAsync(unauthorizedUser.UserEmail, unauthorizedUser.Password));
        }

        [AllureXunit]
        public async void TestAdminGetUsers()
        {
            // Arrange
            uint userID = 0;
            List<User> usersList = new List<User> { UserBuilder.aUser().build() };
            var usersRepositoryStub = new Mock<IUsersRepository>();
            var checkPermissionServiceMock = new Mock<ICheckPermissionService>();
            {
                usersRepositoryStub.Setup(m => m.GetUsersAsync(It.IsAny<uint>(), It.IsAny<uint>())).ReturnsAsync(usersList);
            }

            var sut = new UsersService(checkPermissionServiceMock.Object, usersRepositoryStub.Object);

            // act
            List<User> resultUsers = await sut.AdminGetUsersAsync(userID);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            usersRepositoryStub.Verify(m => m.GetUsersAsync(It.IsAny<uint>(), It.IsAny<uint>()), Times.Once);
            Assert.Equal(usersList, resultUsers);
        }

        [AllureXunit]
        public async void TestAdminAddAutoIncrementUser()
        {
            // Arrange
            uint userID = 0;
            User newUser = UserBuilder.aUser().build();
            var usersRepositoryStub = new Mock<IUsersRepository>();
            var checkPermissionServiceMock = new Mock<ICheckPermissionService>();
            {
                usersRepositoryStub.Setup(m => m.AddUserAutoIncrementAsync(newUser.CardID, newUser.UserEmail, newUser.Password, newUser.Permissions)).ReturnsAsync(newUser.UserID);
            }

            var sut = new UsersService(checkPermissionServiceMock.Object, usersRepositoryStub.Object);

            // act
            uint newUserID = await sut.AdminAddAutoIncrementUserAsync(userID, newUser.CardID, newUser.UserEmail, newUser.Password, newUser.Permissions);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            usersRepositoryStub.Verify(m => m.AddUserAutoIncrementAsync(newUser.CardID, newUser.UserEmail, newUser.Password, newUser.Permissions), Times.Once);
            Assert.Equal(newUser.UserID, newUserID);
        }

        [AllureXunit]
        public async void TestAdminUpdateUser()
        {
            // Arrange
            uint userID = 0;
            User newUser = UserBuilder.aUser().build();
            var usersRepositoryStub = new Mock<IUsersRepository>();
            var checkPermissionServiceMock = new Mock<ICheckPermissionService>();

            var sut = new UsersService(checkPermissionServiceMock.Object, usersRepositoryStub.Object);

            // act
            await sut.AdminUpdateUserAsync(userID, newUser.UserID, newUser.CardID, newUser.UserEmail, newUser.Password, newUser.Permissions);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            usersRepositoryStub.Verify(m => m.UpdateUserByIDAsync(newUser.UserID, newUser.CardID, newUser.UserEmail, newUser.Password, newUser.Permissions), Times.Once);
        }

        [AllureXunit]
        public async void TestAdminDeleteUser()
        {
            // Arrange
            uint userID = 0;
            uint userToDeleteID = 1;
            var usersRepositoryStub = new Mock<IUsersRepository>();
            var checkPermissionServiceMock = new Mock<ICheckPermissionService>();

            var sut = new UsersService(checkPermissionServiceMock.Object, usersRepositoryStub.Object);

            // act
            await sut.AdminDeleteUserAsync(userID, userToDeleteID);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            usersRepositoryStub.Verify(m => m.DeleteUserByIDAsync(userToDeleteID), Times.Once);
        }

        [AllureXunit]
        public async void TestAdminGetUserByIDAsync()
        {
            // Arrange
            uint userID = 0;
            User userToGet = UserBuilder.aUser().build();
            var usersRepositoryStub = new Mock<IUsersRepository>();
            {
                usersRepositoryStub.Setup(m => m.GetUserByIdAsync(userToGet.UserID)).ReturnsAsync(userToGet);
            }
            var checkPermissionServiceMock = new Mock<ICheckPermissionService>();

            var sut = new UsersService(checkPermissionServiceMock.Object, usersRepositoryStub.Object);

            // act
            User userFromService = await sut.AdminGetUserByIDAsync(userID, userToGet.UserID);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            usersRepositoryStub.Verify(m => m.GetUserByIdAsync(userToGet.UserID), Times.Once);
            Assert.Equal(userToGet, userFromService);
        }
    }
}
