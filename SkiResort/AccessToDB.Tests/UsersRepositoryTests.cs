//using System.Threading.Tasks;
//using Xunit;
//using BL.Models;
//using BL.IRepositories;
//using AccessToDB.RepositoriesTarantool;
//using AccessToDB.Exceptions.UserExceptions;
//using System;
//using Allure.Xunit.Attributes;
//using AccessToDB.Tests.ArrangeHelpers;

//// классический подход
////https://xunit.net/docs/shared-context
////Constructor and Dispose
//// When to use: when you want a clean test context for every test
//// (sharing the setup and cleanup code, without sharing the object instance).

//namespace AccessToDB.Tests
//{
//    [AllureSuite("UsersRepositorySuite")]
//    public class UsersRepositoryTests : IDisposable
//    {
//        private IUsersRepository sut { get { return _usersRepository; } }
//        private IUsersRepository _usersRepository;
//        private TarantoolContext _tarantoolContext;

//        public UsersRepositoryTests()
//        {
//            _tarantoolContext = new TarantoolContext("ski_admin:Tty454r293300@localhost:3301");
//            _usersRepository = new TarantoolUsersRepository(_tarantoolContext);
//            CleanUsersTable().GetAwaiter().GetResult();
//        }
//        public void Dispose()
//        {
//            CleanUsersTable().GetAwaiter().GetResult();
//            _tarantoolContext.Dispose();
//        }

//        private async Task CleanUsersTable()
//        {
//            foreach (var user in await _usersRepository.GetUsersAsync())
//            {
//                await _usersRepository.DeleteUserByIDAsync(user.UserID);
//            }
//        }

//        [AllureXunit]
//        [AutoMoqData]
//        public async void TestAddUserAutoIncrementAsyncOk()
//        {
//            // arrange
//            User user = new(1, "A0", true, 1, 1, 1);

//            // act
//            await sut.AddUserAutoIncrementAsync(user.UserName, user.IsOpen, user.SeatsAmount, user.UseringTime);

//            //assert
//            Assert.True(user.EqualWithoutConnectedSlopesAndQueueTime(await sut.GetUserByNameAsync(user.UserName)));
//            Assert.Single(await sut.GetUsersAsync());
//        }

//        [AllureXunitTheory]
//        [AutoMoqData]
//        public async void TestAddUserAsyncFailsWhenUserIdRepeats(User user)
//        {
//            // arrange
//            await sut.AddUserAsync(user.UserID, user.UserName, user.IsOpen, user.SeatsAmount, user.UseringTime, user.QueueTime);

//            // act & assert
//            await Assert.ThrowsAsync<UserAddException>(async () => await sut.AddUserAsync(user.UserID, user.UserName, user.IsOpen, user.SeatsAmount, user.UseringTime, user.QueueTime));

//            //assert
//            Assert.Single(await sut.GetUsersAsync());
//        }

//        [AllureXunit]
//        [AutoMoqData]
//        public async void TestGetUsersAsyncEmpty()
//        {
//            // arrange

//            // act
//            var users = await sut.GetUsersAsync();

//            //assert
//            Assert.Empty(users);
//        }

//        [AllureXunitTheory]
//        [AutoMoqData]
//        public async void TestGetUserByIdAsyncOk(User user)
//        {
//            // arrange
//            await sut.AddUserAsync(user.UserID, user.UserName, user.IsOpen, user.SeatsAmount, user.UseringTime, user.QueueTime);

//            // act
//            var userFromRepository = await sut.GetUserByIdAsync(user.UserID);

//            //assert
//            Assert.Equal(user, userFromRepository);
//        }

//        [AllureXunitTheory]
//        [AutoMoqData]
//        public async void TestGetUserByNameAsyncFailsWhenNameNotExists(string userName)
//        {
//            // arrange

//            // act & assert
//            await Assert.ThrowsAsync<UserNotFoundException>(async () => await sut.GetUserByNameAsync(userName));
//        }

//        [AllureXunitTheory]
//        [AutoMoqData]
//        public async void TestUpdateUserByIDAsyncOk(User user)
//        {
//            // arrange
//            User newUser = new(user.UserID, user.UserName + "smth", !user.IsOpen, user.SeatsAmount, user.UseringTime, user.QueueTime);
//            await sut.AddUserAsync(user.UserID, user.UserName, user.IsOpen, user.SeatsAmount, user.UseringTime, user.QueueTime);

//            // act
//            await sut.UpdateUserByIDAsync(user.UserID, newUser.UserName, newUser.IsOpen, user.SeatsAmount, user.UseringTime);

//            //assert
//            Assert.True(newUser.EqualWithoutConnectedSlopesAndQueueTime(await sut.GetUserByIdAsync(user.UserID)));
//        }

//        [AllureXunitTheory]
//        [AutoMoqData]
//        public async void TestDeleteUserByIDAsyncFailsWhenIDNotExists(uint userID)
//        {
//            // arrange

//            // act & assert
//            await Assert.ThrowsAsync<UserDeleteException>(async () => await sut.DeleteUserByIDAsync(userID));
//        }
//    }
//}


