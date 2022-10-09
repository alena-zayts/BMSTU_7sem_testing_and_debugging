using Moq;
using Xunit;
using BL.Services;
using BL.Models;
using BL.IRepositories;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using BL.Exceptions.UserExceptions;



// лондонский вариант -- изоляция кода от зависимостей
// используется mock для: IUsersRepository, ICheckPermissionService
// не используется AutoMoqData, вместо него прямое создание Mock
// Используется паттерн DataBuilder для генерации объектов для тестов
namespace BL.Tests
{
    internal class UserBuilder
    {

        public static string DEFAULT_EMAIL = "default_email";
        public static string DEFAULT_PASSWORD = "default_password";
        public static PermissionsEnum DEFAULT_PERMISSIONS = PermissionsEnum.UNAUTHORIZED;
        public static uint DEFAULT_CARD_ID = 0;
        public static uint DEFAULT_ID = 0;

        private string email = DEFAULT_EMAIL;
        private string password = DEFAULT_PASSWORD;
        private PermissionsEnum permissions = DEFAULT_PERMISSIONS;
        private uint cardID = DEFAULT_CARD_ID;
        private uint id = DEFAULT_ID;

        private UserBuilder()
        {
        }

        public static UserBuilder aUser()
        {
            return new UserBuilder();
        }

        public UserBuilder withEmail(string email)
        {
            this.email = email;
            return this;
        }
        public UserBuilder withoutEmail()
        {
            this.email = "";
            return this;
        }

        public UserBuilder withPassword(string password)
        {
            this.password = password;
            return this;
        }
        public UserBuilder withoutPassword()
        {
            this.password = "";
            return this;
        }

        public UserBuilder inUnauthorizedRole()
        {
            this.permissions = PermissionsEnum.UNAUTHORIZED;
            return this;
        }

        public UserBuilder inAuthorizedRole()
        {
            this.permissions = PermissionsEnum.AUTHORIZED;
            return this;
        }

        public UserBuilder inSkiPatrolRole()
        {
            this.permissions = PermissionsEnum.SKI_PATROL;
            return this;
        }

        public UserBuilder inAdminRole()
        {
            this.permissions = PermissionsEnum.ADMIN;
            return this;
        }

        public UserBuilder inRole(PermissionsEnum permissions)
        {
            this.permissions = permissions;
            return this;
        }

        public UserBuilder but()
        {
            return UserBuilder
                    .aUser();
        }

        public User build()
        {
            return new User(id, cardID, email, password, permissions);
        }
    }


    public class UsersServiceTests
    {
        [Fact]
        public async void TestRegisterOk()
        {
            // Arrange
            UserBuilder userBuilder = UserBuilder.aUser();
            User unauthorizedUser = userBuilder.inUnauthorizedRole().build();
            User authorizedUser = userBuilder.inAuthorizedRole().build();
            var usersRepositoryMock = new Mock<IUsersRepository>();
            {
                usersRepositoryMock.Setup(m => m.CheckUserEmailExistsAsync(unauthorizedUser.UserEmail)).ReturnsAsync(false);
                usersRepositoryMock.Setup(m => m.AddUserAutoIncrementAsync(unauthorizedUser.CardID, unauthorizedUser.UserEmail, unauthorizedUser.Password, authorizedUser.Permissions)).ReturnsAsync(unauthorizedUser.UserID);
                usersRepositoryMock.Setup(m => m.GetUserByEmailAsync(unauthorizedUser.UserEmail)).ReturnsAsync(authorizedUser);
            }
            var checkPermissionServiceMock = new Mock<ICheckPermissionService>();
            var sut = new UsersService(checkPermissionServiceMock.Object, usersRepositoryMock.Object);


            // act
            User resultUser = await sut.RegisterAsync(unauthorizedUser.CardID, unauthorizedUser.UserEmail, unauthorizedUser.Password);

            //assert
            usersRepositoryMock.Verify(m => m.CheckUserEmailExistsAsync(unauthorizedUser.UserEmail), Times.Once);
            usersRepositoryMock.Verify(m => m.AddUserAutoIncrementAsync(unauthorizedUser.CardID, unauthorizedUser.UserEmail, unauthorizedUser.Password, authorizedUser.Permissions), Times.Once);
            Assert.Equal(authorizedUser, resultUser);
        }

        [Fact]
        public async void TestRegisterUserWithNoEmail()
        {
            // Arrange
            User unauthorizedUser = UserBuilder.aUser().inUnauthorizedRole().withoutEmail().build();
            var usersRepositoryMock = new Mock<IUsersRepository>();
            var checkPermissionServiceMock = new Mock<ICheckPermissionService>();
            var sut = new UsersService(checkPermissionServiceMock.Object, usersRepositoryMock.Object);

            // act & assert
            _ = Assert.ThrowsAsync<UserRegistrationException>(async () => await sut.RegisterAsync(unauthorizedUser.CardID, unauthorizedUser.UserEmail, unauthorizedUser.Password));
        }

        [Fact]
        public async void TestRegisterUserWithNoPassword()
        {
            // Arrange
            User unauthorizedUser = UserBuilder.aUser().inUnauthorizedRole().withoutPassword().build();
            var usersRepositoryMock = new Mock<IUsersRepository>();
            var checkPermissionServiceMock = new Mock<ICheckPermissionService>();
            var sut = new UsersService(checkPermissionServiceMock.Object, usersRepositoryMock.Object);

            // act & assert
            _ = Assert.ThrowsAsync<UserRegistrationException>(async () => await sut.RegisterAsync(unauthorizedUser.CardID, unauthorizedUser.UserEmail, unauthorizedUser.Password));
        }

        [Fact]
        public async void TestRegisterUserWithRepeatedEmail()
        {
            // Arrange
            User unauthorizedUser = UserBuilder.aUser().inUnauthorizedRole().build();
            var usersRepositoryMock = new Mock<IUsersRepository>();
            {
                usersRepositoryMock.Setup(m => m.CheckUserEmailExistsAsync(unauthorizedUser.UserEmail)).ReturnsAsync(true);
            }
            var checkPermissionServiceMock = new Mock<ICheckPermissionService>();
            var sut = new UsersService(checkPermissionServiceMock.Object, usersRepositoryMock.Object);

            // act & assert
            _ = Assert.ThrowsAsync<UserRegistrationException>(async () => await sut.RegisterAsync(unauthorizedUser.CardID, unauthorizedUser.UserEmail, unauthorizedUser.Password));
        }

        [Fact]
        public async void TestLogInOk()
        {
            // Arrange
            UserBuilder userBuilder = UserBuilder.aUser();
            User unauthorizedUser = userBuilder.inUnauthorizedRole().build();
            User registeredUser = userBuilder.inAuthorizedRole().build();
            var usersRepositoryMock = new Mock<IUsersRepository>();
            {
                usersRepositoryMock.Setup(m => m.GetUserByEmailAsync(unauthorizedUser.UserEmail)).ReturnsAsync(registeredUser);
            }
            var checkPermissionServiceMock = new Mock<ICheckPermissionService>();
            var sut = new UsersService(checkPermissionServiceMock.Object, usersRepositoryMock.Object);


            // act
            User resultUser = await sut.LogInAsync(unauthorizedUser.UserEmail, unauthorizedUser.Password);

            //assert
            usersRepositoryMock.Verify(m => m.GetUserByEmailAsync(unauthorizedUser.UserEmail), Times.Once);
            Assert.Equal(registeredUser, resultUser);
        }

        [Fact]
        public async void TestLogInWithWrongPassword()
        {
            // Arrange
            UserBuilder userBuilder = UserBuilder.aUser();
            User unauthorizedUser = userBuilder.inUnauthorizedRole().withPassword("321").build();
            User registeredUser = userBuilder.inAuthorizedRole().withPassword("123").build();
            var usersRepositoryMock = new Mock<IUsersRepository>();
            {
                usersRepositoryMock.Setup(m => m.GetUserByEmailAsync(unauthorizedUser.UserEmail)).ReturnsAsync(registeredUser);
            }
            var checkPermissionServiceMock = new Mock<ICheckPermissionService>();
            var sut = new UsersService(checkPermissionServiceMock.Object, usersRepositoryMock.Object);

            // act & assert
            _ = Assert.ThrowsAsync<UserAuthorizationException>(async () => await sut.LogInAsync(unauthorizedUser.UserEmail, unauthorizedUser.Password));
        }

        [Fact]
        public async void TestAdminGetUsers()
        {
            // Arrange
            uint userID = 0;
            List<User> usersList = new List<User> { UserBuilder.aUser().build() };
            var usersRepositoryMock = new Mock<IUsersRepository>();
            var checkPermissionServiceMock = new Mock<ICheckPermissionService>();
            {
                usersRepositoryMock.Setup(m => m.GetUsersAsync(It.IsAny<uint>(), It.IsAny<uint>())).ReturnsAsync(usersList);
            }
            
            var sut = new UsersService(checkPermissionServiceMock.Object, usersRepositoryMock.Object);

            // act
            List<User> resultUsers = await sut.AdminGetUsersAsync(userID);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            usersRepositoryMock.Verify(m => m.GetUsersAsync(It.IsAny<uint>(), It.IsAny<uint>()), Times.Once);
            Assert.Equal(usersList, resultUsers);
        }

        [Fact]
        public async void TestAdminAddAutoIncrementUser()
        {
            // Arrange
            uint userID = 0;
            User newUser =  UserBuilder.aUser().build();
            var usersRepositoryMock = new Mock<IUsersRepository>();
            var checkPermissionServiceMock = new Mock<ICheckPermissionService>();
            {
                usersRepositoryMock.Setup(m => m.AddUserAutoIncrementAsync(newUser.CardID, newUser.UserEmail, newUser.Password, newUser.Permissions)).ReturnsAsync(newUser.UserID);
            }

            var sut = new UsersService(checkPermissionServiceMock.Object, usersRepositoryMock.Object);

            // act
            uint newUserID = await sut.AdminAddAutoIncrementUserAsync(userID, newUser.CardID, newUser.UserEmail, newUser.Password, newUser.Permissions);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            usersRepositoryMock.Verify(m => m.AddUserAutoIncrementAsync(newUser.CardID, newUser.UserEmail, newUser.Password, newUser.Permissions), Times.Once);
            Assert.Equal(newUser.UserID, newUserID);
        }

        [Fact]
        public async void TestAdminUpdateUser()
        {
            // Arrange
            uint userID = 0;
            User newUser = UserBuilder.aUser().build();
            var usersRepositoryMock = new Mock<IUsersRepository>();
            var checkPermissionServiceMock = new Mock<ICheckPermissionService>();

            var sut = new UsersService(checkPermissionServiceMock.Object, usersRepositoryMock.Object);

            // act
            await sut.AdminUpdateUserAsync(userID, newUser.UserID, newUser.CardID, newUser.UserEmail, newUser.Password, newUser.Permissions);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            usersRepositoryMock.Verify(m => m.UpdateUserByIDAsync(newUser.UserID, newUser.CardID, newUser.UserEmail, newUser.Password, newUser.Permissions), Times.Once);
        }

        [Fact]
        public async void TestAdminDeleteUser()
        {
            // Arrange
            uint userID = 0;
            uint userToDeleteID = 1;
            var usersRepositoryMock = new Mock<IUsersRepository>();
            var checkPermissionServiceMock = new Mock<ICheckPermissionService>();

            var sut = new UsersService(checkPermissionServiceMock.Object, usersRepositoryMock.Object);

            // act
            await sut.AdminDeleteUserAsync(userID, userToDeleteID);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            usersRepositoryMock.Verify(m => m.DeleteUserByIDAsync(userToDeleteID), Times.Once);
        }

        [Fact]
        public async void TestAdminGetUserByIDAsync()
        {
            // Arrange
            uint userID = 0;
            User userToGet = UserBuilder.aUser().build();
            var usersRepositoryMock = new Mock<IUsersRepository>();
            {
                usersRepositoryMock.Setup(m => m.GetUserByIdAsync(userToGet.UserID)).ReturnsAsync(userToGet);
            }
            var checkPermissionServiceMock = new Mock<ICheckPermissionService>();

            var sut = new UsersService(checkPermissionServiceMock.Object, usersRepositoryMock.Object);

            // act
            User userFromService = await sut.AdminGetUserByIDAsync(userID, userToGet.UserID);

            //assert
            checkPermissionServiceMock.Verify(m => m.CheckPermissionsAsync(userID, It.IsAny<string>()), Times.Once);
            usersRepositoryMock.Verify(m => m.GetUserByIdAsync(userToGet.UserID), Times.Once);
            Assert.Equal(userToGet, userFromService);
        }
    }
}
