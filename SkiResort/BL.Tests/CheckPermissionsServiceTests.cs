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
using BL.Tests.ArrangeHelpers;

// лондонский вариант -- изоляция кода от зависимостей
// используется stub для: IUsersRepository (вызовы и взаимодействия,  которые исполняются SUT к зависимым объектам, чтобы запросить и получить данные)
// не используется AutoMoqData, вместо него прямое создание Mock и InlineData
// Используется Fabric (Object Mother) для генерации объектов для тестов

namespace BL.Tests
{

    [AllureSuite("CheckPermissionsServiceSuite")]
    public class CheckPermissionsServiceTests
    {
        [AllureXunit]
        public async void UnauthorizedHasNoAccessToAdminFunctions()
        {
            // arrange
            string functionName = "smthWithAdminProperty";
            User user = UsersObjectMother.UnauthorizedUser();
            var usersRepositoryStub = new Mock<IUsersRepository>();
            {
                usersRepositoryStub.Setup(m => m.GetUserByIdAsync(user.UserID)).ReturnsAsync(user);
            }
            var sut = new CheckPermissionsService(usersRepositoryStub.Object);


            // act & assert
            _ = Assert.ThrowsAsync<PermissionException>(async () => await sut.CheckPermissionsAsync(user.UserID, functionName));

            //assert
            usersRepositoryStub.Verify(m => m.GetUserByIdAsync(user.UserID), Times.Once);
        }

        [AllureXunit]
        public async void AuthorizedHasNoAccessToAdminFunctions()
        {
            // arrange
            string functionName = "smthWithAdminProperty";
            User user = UsersObjectMother.AuthorizedUser();
            var usersRepositoryStub = new Mock<IUsersRepository>();
            {
                usersRepositoryStub.Setup(m => m.GetUserByIdAsync(user.UserID)).ReturnsAsync(user);
            }
            var sut = new CheckPermissionsService(usersRepositoryStub.Object);


            // act & assert
            _ = Assert.ThrowsAsync<PermissionException>(async () => await sut.CheckPermissionsAsync(user.UserID, functionName));

            //assert
            usersRepositoryStub.Verify(m => m.GetUserByIdAsync(user.UserID), Times.Once);
        }

        [AllureXunit]
        public async void SkiPatrolHasNoAccessToAdminFunctions()
        {
            // arrange
            string functionName = "smthWithAdminProperty";
            User user = UsersObjectMother.SkiPatrolUser();
            var usersRepositoryStub = new Mock<IUsersRepository>();
            {
                usersRepositoryStub.Setup(m => m.GetUserByIdAsync(user.UserID)).ReturnsAsync(user);
            }
            var sut = new CheckPermissionsService(usersRepositoryStub.Object);


            // act & assert
            _ = Assert.ThrowsAsync<PermissionException>(async () => await sut.CheckPermissionsAsync(user.UserID, functionName));

            //assert
            usersRepositoryStub.Verify(m => m.GetUserByIdAsync(user.UserID), Times.Once);
        }

        [AllureXunit]
        public async void AdminHasAccessToAllFunctions()
        {
            // arrange
            string functionName = "any";
            User user = UsersObjectMother.AdminUser();
            var usersRepositoryStub = new Mock<IUsersRepository>();
            {
                usersRepositoryStub.Setup(m => m.GetUserByIdAsync(user.UserID)).ReturnsAsync(user);
            }
            var sut = new CheckPermissionsService(usersRepositoryStub.Object);


            // act 
            var task = sut.CheckPermissionsAsync(user.UserID, functionName);

            //assert
            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            usersRepositoryStub.Verify(m => m.GetUserByIdAsync(user.UserID), Times.Once);
        }

        [AllureXunitTheory]
        [InlineData("MarkMessageReadByUserAsync")]
        [InlineData("GetMessagesAsync")]
        [InlineData("GetLiftsSlopesInfoAsync")]
        public async void UnauthorizedHasNoAccessToAdminSkiPatrolFunctions(string functionName)
        {
            // arrange
            User user = UsersObjectMother.UnauthorizedUser();
            var usersRepositoryStub = new Mock<IUsersRepository>();
            {
                usersRepositoryStub.Setup(m => m.GetUserByIdAsync(user.UserID)).ReturnsAsync(user);
            }
            var sut = new CheckPermissionsService(usersRepositoryStub.Object);


            // act & assert
            _ = Assert.ThrowsAsync<PermissionException>(async () => await sut.CheckPermissionsAsync(user.UserID, functionName));

            //assert
            usersRepositoryStub.Verify(m => m.GetUserByIdAsync(user.UserID), Times.Once);
        }

        [AllureXunitTheory]
        [InlineData("MarkMessageReadByUserAsync")]
        [InlineData("GetMessagesAsync")]
        [InlineData("GetLiftsSlopesInfoAsync")]
        public async void AuthorizedHasNoAccessToAdminSkiPatrolFunctions(string functionName)
        {
            // arrange
            User user = UsersObjectMother.AuthorizedUser();
            var usersRepositoryStub = new Mock<IUsersRepository>();
            {
                usersRepositoryStub.Setup(m => m.GetUserByIdAsync(user.UserID)).ReturnsAsync(user);
            }
            var sut = new CheckPermissionsService(usersRepositoryStub.Object);


            // act & assert
            _ = Assert.ThrowsAsync<PermissionException>(async () => await sut.CheckPermissionsAsync(user.UserID, functionName));

            //assert
            usersRepositoryStub.Verify(m => m.GetUserByIdAsync(user.UserID), Times.Once);
        }

        [AllureXunitTheory]
        [InlineData("MarkMessageReadByUserAsync")]
        [InlineData("GetMessagesAsync")]
        [InlineData("GetLiftsSlopesInfoAsync")]
        public async void SkiPatrolHasAccessToAdminSkiPatrolFunctions(string functionName)
        {
            // arrange
            User user = UsersObjectMother.SkiPatrolUser();
            var usersRepositoryStub = new Mock<IUsersRepository>();
            {
                usersRepositoryStub.Setup(m => m.GetUserByIdAsync(user.UserID)).ReturnsAsync(user);
            }
            var sut = new CheckPermissionsService(usersRepositoryStub.Object);


            // act 
            var task = sut.CheckPermissionsAsync(user.UserID, functionName);

            //assert
            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            usersRepositoryStub.Verify(m => m.GetUserByIdAsync(user.UserID), Times.Once);
        }

        [AllureXunitTheory]
        [InlineData("SendMessageAsync")]
        [InlineData("LogOutAsync")]
        public async void UnauthorizedHasNoAccessToAuthorizedFunctions(string functionName)
        {
            // arrange
            User user = UsersObjectMother.UnauthorizedUser();
            var usersRepositoryStub = new Mock<IUsersRepository>();
            {
                usersRepositoryStub.Setup(m => m.GetUserByIdAsync(user.UserID)).ReturnsAsync(user);
            }
            var sut = new CheckPermissionsService(usersRepositoryStub.Object);


            // act & assert
            _ = Assert.ThrowsAsync<PermissionException>(async () => await sut.CheckPermissionsAsync(user.UserID, functionName));

            //assert
            usersRepositoryStub.Verify(m => m.GetUserByIdAsync(user.UserID), Times.Once);
        }

        [AllureXunitTheory]
        [InlineData("SendMessageAsync")]
        [InlineData("LogOutAsync")]
        public async void AuthorizedHasAccessToAuthorizedFunctions(string functionName)
        {
            // arrange
            User user = UsersObjectMother.AuthorizedUser();
            var usersRepositoryStub = new Mock<IUsersRepository>();
            {
                usersRepositoryStub.Setup(m => m.GetUserByIdAsync(user.UserID)).ReturnsAsync(user);
            }
            var sut = new CheckPermissionsService(usersRepositoryStub.Object);


            // act 
            var task = sut.CheckPermissionsAsync(user.UserID, functionName);

            //assert
            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            usersRepositoryStub.Verify(m => m.GetUserByIdAsync(user.UserID), Times.Once);
        }
    }
}
