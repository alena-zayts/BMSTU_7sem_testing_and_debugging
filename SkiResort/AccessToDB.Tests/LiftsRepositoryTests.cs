using System.Threading.Tasks;
using Xunit;
using BL.Models;
using BL.IRepositories;
using AccessToDB.RepositoriesTarantool;
using AccessToDB.Exceptions.LiftExceptions;
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
    [AllureSuite("LiftsRepositorySuite")]
    public class LiftsRepositoryTests : IDisposable
    {
        private ILiftsRepository sut { get { return _liftsRepository; } }
        private ILiftsRepository _liftsRepository;
        private TarantoolContext _tarantoolContext;

        public LiftsRepositoryTests()
        {
            _tarantoolContext = new TarantoolContext("ski_admin:Tty454r293300@localhost:3301");
            _liftsRepository = new TarantoolLiftsRepository(_tarantoolContext);
            CleanLiftsTable().GetAwaiter().GetResult();
        }
        public void Dispose()
        {
            CleanLiftsTable().GetAwaiter().GetResult();
            _tarantoolContext.Dispose();
        }

        private async Task CleanLiftsTable()
        {
            foreach (var lift in await _liftsRepository.GetLiftsAsync())
            {
                await _liftsRepository.DeleteLiftByIDAsync(lift.LiftID);
            }
        }

        [AllureXunit]
        [AutoMoqData]
        public async void TestAddLiftAutoIncrementAsyncOk()
        {
            // arrange
            Lift lift = new(1, "A0", true, 1, 1, 1);

            // act
            await sut.AddLiftAutoIncrementAsync(lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime);

            //assert
            Assert.True(lift.EqualWithoutConnectedSlopesAndQueueTime(await sut.GetLiftByNameAsync(lift.LiftName)));
            Assert.Single(await sut.GetLiftsAsync());
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAddLiftAsyncFailsWhenLiftIdRepeats(Lift lift)
        {
            // arrange
            await sut.AddLiftAsync(lift.LiftID, lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime, lift.QueueTime);

            // act & assert
            await Assert.ThrowsAsync<LiftAddException>(async () => await sut.AddLiftAsync(lift.LiftID, lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime, lift.QueueTime));

            //assert
            Assert.Single(await sut.GetLiftsAsync());
        }

        [AllureXunit]
        [AutoMoqData]
        public async void TestGetLiftsAsyncEmpty()
        {
            // arrange

            // act
            var lifts = await sut.GetLiftsAsync();

            //assert
            Assert.Empty(lifts);
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestGetLiftByIdAsyncOk(Lift lift)
        {
            // arrange
            await sut.AddLiftAsync(lift.LiftID, lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime, lift.QueueTime);

            // act
            var liftFromRepository = await sut.GetLiftByIdAsync(lift.LiftID);

            //assert
            Assert.Equal(lift, liftFromRepository);
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestGetLiftByNameAsyncFailsWhenNameNotExists(string liftName)
        {
            // arrange

            // act & assert
            await Assert.ThrowsAsync<LiftNotFoundException>(async () => await sut.GetLiftByNameAsync(liftName));
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestUpdateLiftByIDAsyncOk(Lift lift)
        {
            // arrange
            Lift newLift = new(lift.LiftID, lift.LiftName + "smth", !lift.IsOpen, lift.SeatsAmount, lift.LiftingTime, lift.QueueTime);
            await sut.AddLiftAsync(lift.LiftID, lift.LiftName, lift.IsOpen, lift.SeatsAmount, lift.LiftingTime, lift.QueueTime);

            // act
            await sut.UpdateLiftByIDAsync(lift.LiftID, newLift.LiftName, newLift.IsOpen, lift.SeatsAmount, lift.LiftingTime);

            //assert
            Assert.True(newLift.EqualWithoutConnectedSlopesAndQueueTime(await sut.GetLiftByIdAsync(lift.LiftID)));
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestDeleteLiftByIDAsyncFailsWhenIDNotExists(uint liftID)
        {
            // arrange

            // act & assert
            await Assert.ThrowsAsync<LiftDeleteException>(async () => await sut.DeleteLiftByIDAsync(liftID));
        }
    }
}


