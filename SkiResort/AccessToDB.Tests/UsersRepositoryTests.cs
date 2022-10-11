using System.Threading.Tasks;
using Xunit;
using BL.Models;
using BL.IRepositories;
using AccessToDB.RepositoriesTarantool;
using AccessToDB.Exceptions.UserExceptions;
using System;
using Allure.Xunit.Attributes;
using AccessToDB.Tests.ArrangeHelpers;

// классический подход
//https://xunit.net/docs/shared-context
//Constructor and Dispose
// When to use: when you want a clean test context for every test
// (sharing the setup and cleanup code, without sharing the object instance).

namespace AccessToDB.Tests
{
    [AllureSuite("UsersRepositorySuite")]
    public class UsersRepositoryTests : IDisposable
    {
        private IUsersRepository sut { get { return _usersRepository; } }
        private IUsersRepository _usersRepository;
        private TarantoolContext _tarantoolContext;

        public UsersRepositoryTests()
        {
            _tarantoolContext = new TarantoolContext("ski_admin:Tty454r293300@localhost:3301");
            _usersRepository = new TarantoolUsersRepository(_tarantoolContext);
            CleanUsersTable().GetAwaiter().GetResult();
        }
        public void Dispose()
        {
            CleanUsersTable().GetAwaiter().GetResult();
            _tarantoolContext.Dispose();
        }

        private async Task CleanUsersTable()
        {
            foreach (var user in await _usersRepository.GetUsersAsync())
            {
                await _usersRepository.DeleteUserByIDAsync(user.UserID);
            }
        }

        [AllureXunit]
        [AutoMoqData]
        public async void TestAddUserAutoIncrementAsyncOk()
        {
            // arrange
            User user = new(1, 1, "tmp", "tmp", PermissionsEnum.ADMIN);

            // act
            await sut.AddUserAutoIncrementAsync(user.CardID, user.UserEmail, user.Password, user.Permissions);

            //assert
            Assert.Equal(user, await sut.GetUserByIdAsync(user.UserID));
            Assert.Single(await sut.GetUsersAsync());
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAddUserAsyncFailsWhenUserIdRepeats(User user)
        {
            // arrange
            await sut.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions);

            // act & assert
            await Assert.ThrowsAsync<UserAddException>(async () => await sut.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions));

            //assert
            Assert.Single(await sut.GetUsersAsync());
        }

        [AllureXunit]
        [AutoMoqData]
        public async void TestGetUsersAsyncEmpty()
        {
            // arrange

            // act
            var users = await sut.GetUsersAsync();

            //assert
            Assert.Empty(users);
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestGetUserByIdAsyncOk(User user)
        {
            // arrange
            await sut.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions);

            // act
            var userFromRepository = await sut.GetUserByIdAsync(user.UserID);

            //assert
            Assert.Equal(user, userFromRepository);
        }


        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestUpdateUserByIDAsyncOk(User user)
        {
            // arrange
            User newUser = new(user.UserID, user.CardID + 5, user.UserEmail + "smth", user.Password + "more", user.Permissions);
            await sut.AddUserAsync(user.UserID, user.CardID, user.UserEmail, user.Password, user.Permissions);

            // act
            await sut.UpdateUserByIDAsync(user.UserID, newUser.CardID, newUser.UserEmail, newUser.Password, newUser.Permissions);

            //assert
            Assert.Equal(newUser, await sut.GetUserByIdAsync(user.UserID));
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestDeleteUserByIDAsyncFailsWhenIDNotExists(uint userID)
        {
            // arrange

            // act & assert
            await Assert.ThrowsAsync<UserDeleteException>(async () => await sut.DeleteUserByIDAsync(userID));
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestGetUserByEmailAsyncFailsWhenEmailNotExists(string email)
        {
            // arrange

            // act & assert
            await Assert.ThrowsAsync<UserNotFoundException>(async () => await sut.GetUserByEmailAsync(email));
        }

        //Task<bool> CheckUserIdExistsAsync(uint userID);
        //Task<bool> CheckUserEmailExistsAsync(string userEmail);
    }
}


