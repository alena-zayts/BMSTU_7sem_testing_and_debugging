using Moq;
using Xunit;
using BL.Services;
using BL.Models;
using BL.IRepositories;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using AutoFixture.AutoMoq;
using BL.Exceptions.PermissionExceptions;
using System.Runtime.InteropServices;
using System;
using Allure.Xunit.Attributes;

// лондонский вариант -- изоляция кода от зависимостей
// используется mock для: IUsersRepository
// не используется AutoMoqData, вместо него прямое создание Mock и InlineData
// Используется Fabric (Object Mother) для генерации объектов для тестов

namespace BL.Tests
{
    internal class UsersObjectMother
    {

        public static User UnauthorizedUser()
        {
            return new User(1, 0, "", "", PermissionsEnum.UNAUTHORIZED);
        }

        public static User AuthorizedUser()
        {
            return new User(2, 2, "authorized_user_email", "authorized_user_password", PermissionsEnum.AUTHORIZED);
        }

        public static User SkiPatrolUser()
        {
            return new User(3, 3, "skipatrol_user_email", "skipatrol_user_password", PermissionsEnum.SKI_PATROL);
        }

        public static User AdminUser()
        {
            return new User(4, 1, "admin_user_email", "admin_user_password", PermissionsEnum.ADMIN);
        }

    }

    [AllureSuite("CheckPermissionsServiceSuite")]
    public class CheckPermissionsServiceTests
    {
        [AllureXunit]
        public async void UnauthorizedHasNoAccessToAdminFunctions()
        {
            //Console.WriteLine(System.Runtime.InteropServices.Des);
            // arrange
            string functionName = "smthWithAdminProperty";
            User user = UsersObjectMother.UnauthorizedUser();
            var usersRepositoryMock = new Mock<IUsersRepository>();
            {
                usersRepositoryMock.Setup(m => m.GetUserByIdAsync(user.UserID)).ReturnsAsync(user);
            }
            var sut = new CheckPermissionsService(usersRepositoryMock.Object);


            // act & assert
            _ = Assert.ThrowsAsync<PermissionException>(async () => await sut.CheckPermissionsAsync(user.UserID, functionName));

            //assert
            usersRepositoryMock.Verify(m => m.GetUserByIdAsync(user.UserID), Times.Once);
        }

        [AllureXunit]
        public async void AuthorizedHasNoAccessToAdminFunctions()
        {
            // arrange
            string functionName = "smthWithAdminProperty";
            User user = UsersObjectMother.AuthorizedUser();
            var usersRepositoryMock = new Mock<IUsersRepository>();
            {
                usersRepositoryMock.Setup(m => m.GetUserByIdAsync(user.UserID)).ReturnsAsync(user);
            }
            var sut = new CheckPermissionsService(usersRepositoryMock.Object);


            // act & assert
            _ = Assert.ThrowsAsync<PermissionException>(async () => await sut.CheckPermissionsAsync(user.UserID, functionName));

            //assert
            usersRepositoryMock.Verify(m => m.GetUserByIdAsync(user.UserID), Times.Once);
        }

        [AllureXunit]
        public async void SkiPatrolHasNoAccessToAdminFunctions()
        {
            // arrange
            string functionName = "smthWithAdminProperty";
            User user = UsersObjectMother.SkiPatrolUser();
            var usersRepositoryMock = new Mock<IUsersRepository>();
            {
                usersRepositoryMock.Setup(m => m.GetUserByIdAsync(user.UserID)).ReturnsAsync(user);
            }
            var sut = new CheckPermissionsService(usersRepositoryMock.Object);


            // act & assert
            _ = Assert.ThrowsAsync<PermissionException>(async () => await sut.CheckPermissionsAsync(user.UserID, functionName));

            //assert
            usersRepositoryMock.Verify(m => m.GetUserByIdAsync(user.UserID), Times.Once);
        }

        [AllureXunit]
        public async void AdminHasAccessToAllFunctions()
        {
            // arrange
            string functionName = "any";
            User user = UsersObjectMother.AdminUser();
            var usersRepositoryMock = new Mock<IUsersRepository>();
            {
                usersRepositoryMock.Setup(m => m.GetUserByIdAsync(user.UserID)).ReturnsAsync(user);
            }
            var sut = new CheckPermissionsService(usersRepositoryMock.Object);


            // act 
            var task = sut.CheckPermissionsAsync(user.UserID, functionName);

            //assert
            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            usersRepositoryMock.Verify(m => m.GetUserByIdAsync(user.UserID), Times.Once);
        }

        [AllureXunitTheory]
        [InlineData("MarkMessageReadByUserAsync")]
        [InlineData("GetMessagesAsync")]
        [InlineData("GetLiftsSlopesInfoAsync")]
        public async void UnauthorizedHasNoAccessToAdminSkiPatrolFunctions(string functionName)
        {
            // arrange
            User user = UsersObjectMother.UnauthorizedUser();
            var usersRepositoryMock = new Mock<IUsersRepository>();
            {
                usersRepositoryMock.Setup(m => m.GetUserByIdAsync(user.UserID)).ReturnsAsync(user);
            }
            var sut = new CheckPermissionsService(usersRepositoryMock.Object);


            // act & assert
            _ = Assert.ThrowsAsync<PermissionException>(async () => await sut.CheckPermissionsAsync(user.UserID, functionName));

            //assert
            usersRepositoryMock.Verify(m => m.GetUserByIdAsync(user.UserID), Times.Once);
        }

        [AllureXunitTheory]
        [InlineData("MarkMessageReadByUserAsync")]
        [InlineData("GetMessagesAsync")]
        [InlineData("GetLiftsSlopesInfoAsync")]
        public async void AuthorizedHasNoAccessToAdminSkiPatrolFunctions(string functionName)
        {
            // arrange
            User user = UsersObjectMother.AuthorizedUser();
            var usersRepositoryMock = new Mock<IUsersRepository>();
            {
                usersRepositoryMock.Setup(m => m.GetUserByIdAsync(user.UserID)).ReturnsAsync(user);
            }
            var sut = new CheckPermissionsService(usersRepositoryMock.Object);


            // act & assert
            _ = Assert.ThrowsAsync<PermissionException>(async () => await sut.CheckPermissionsAsync(user.UserID, functionName));

            //assert
            usersRepositoryMock.Verify(m => m.GetUserByIdAsync(user.UserID), Times.Once);
        }

        [AllureXunitTheory]
        [InlineData("MarkMessageReadByUserAsync")]
        [InlineData("GetMessagesAsync")]
        [InlineData("GetLiftsSlopesInfoAsync")]
        public async void SkiPatrolHasAccessToAdminSkiPatrolFunctions(string functionName)
        {
            // arrange
            User user = UsersObjectMother.SkiPatrolUser();
            var usersRepositoryMock = new Mock<IUsersRepository>();
            {
                usersRepositoryMock.Setup(m => m.GetUserByIdAsync(user.UserID)).ReturnsAsync(user);
            }
            var sut = new CheckPermissionsService(usersRepositoryMock.Object);


            // act 
            var task = sut.CheckPermissionsAsync(user.UserID, functionName);

            //assert
            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            usersRepositoryMock.Verify(m => m.GetUserByIdAsync(user.UserID), Times.Once);
        }

        [AllureXunitTheory]
        [InlineData("SendMessageAsync")]
        [InlineData("LogOutAsync")]
        public async void UnauthorizedHasNoAccessToAuthorizedFunctions(string functionName)
        {
            // arrange
            User user = UsersObjectMother.UnauthorizedUser();
            var usersRepositoryMock = new Mock<IUsersRepository>();
            {
                usersRepositoryMock.Setup(m => m.GetUserByIdAsync(user.UserID)).ReturnsAsync(user);
            }
            var sut = new CheckPermissionsService(usersRepositoryMock.Object);


            // act & assert
            _ = Assert.ThrowsAsync<PermissionException>(async () => await sut.CheckPermissionsAsync(user.UserID, functionName));

            //assert
            usersRepositoryMock.Verify(m => m.GetUserByIdAsync(user.UserID), Times.Once);
        }

        [AllureXunitTheory]
        [InlineData("SendMessageAsync")]
        [InlineData("LogOutAsync")]
        public async void AuthorizedHasAccessToAuthorizedFunctions(string functionName)
        {
            // arrange
            User user = UsersObjectMother.AuthorizedUser();
            var usersRepositoryMock = new Mock<IUsersRepository>();
            {
                usersRepositoryMock.Setup(m => m.GetUserByIdAsync(user.UserID)).ReturnsAsync(user);
            }
            var sut = new CheckPermissionsService(usersRepositoryMock.Object);


            // act 
            var task = sut.CheckPermissionsAsync(user.UserID, functionName);

            //assert
            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            usersRepositoryMock.Verify(m => m.GetUserByIdAsync(user.UserID), Times.Once);
        }
    }
}
