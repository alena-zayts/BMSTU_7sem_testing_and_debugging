//using System.Threading.Tasks;
//using Xunit;
//using BL.Models;
//using BL.IRepositories;
//using AccessToDB.RepositoriesTarantool;
//using AccessToDB.Exceptions.LiftExceptions;
//using System;
//using Allure.Xunit.Attributes;
//using AccessToDB.Tests.ArrangeHelpers;
//using Moq;
//using AutoFixture.Xunit2;
//using AccessToDB.Tests.FakeRepositories;

//// лондонский подход -- репозитории ILiftsRepository и ISlopesRepository -- Fake-и
////https://xunit.net/docs/shared-context
////Constructor and Dispose
//// When to use: when you want a clean test context for every test
//// (sharing the setup and cleanup code, without sharing the object instance).

//namespace AccessToDB.Tests
//{
//    [AllureSuite("LiftsSlopesRepositorySuite")]
//    public class LiftsSlopesRepositoryTests : IDisposable
//    {
//        private ILiftsSlopesRepository sut { get { return _liftsSlopesRepository; } }
//        private ILiftsRepository _liftsRepository;
//        private ISlopesRepository _slopesRepository;
//        private ILiftsSlopesRepository _liftsSlopesRepository;
//        private TarantoolContext _tarantoolContext;

//        public LiftsSlopesRepositoryTests()
//        {
//            _tarantoolContext = new TarantoolContext("ski_admin:Tty454r293300@localhost:3301");
//            _liftsSlopesRepository = new TarantoolLiftsSlopesRepository(_tarantoolContext);
//            _liftsRepository = new FakeLiftsRepository();
//            _slopesRepository = new FakeSlopesRepository();
//            CleanLiftsSlopesTable().GetAwaiter().GetResult();
//        }
//        public void Dispose()
//        {
//            CleanLiftsSlopesTable().GetAwaiter().GetResult();
//            _tarantoolContext.Dispose();
//        }

//        private async Task CleanLiftsSlopesTable()
//        {
//            foreach (var lift in await _liftsRepository.GetLiftsAsync())
//            {
//                await _liftsRepository.DeleteLiftByIDAsync(lift.LiftID);
//            }
//        }

//        [AllureXunit]
//        [AutoMoqData]
//        public async void TestAddLiftSlopeAutoIncrementAsyncOk()
//        {
//            // arrange
//            LiftSlope liftSlope = new(1, 1, 1);

//            // act
//            await sut.AddLiftSlopeAutoIncrementAsync(liftSlope.LiftID, liftSlope.SlopeID);

//            //assert
//            Assert.Equal(liftSlope, await sut.GetLiftSlopeByIdAsync(liftSlope.RecordID));
//            Assert.Single(await sut.GetLiftsSlopesAsync());
//        }

//        [AllureXunitTheory]
//        [AutoMoqData]
//        public async void TestAddLiftSlopeAsyncFailsWhenLiftSlopeIdRepeats(LiftSlope liftSlope)
//        {
//            // arrange
//            await sut.AddLiftSlopeAsync(liftSlope.LiftSlopeID, liftSlope.LiftSlopeName, liftSlope.IsOpen, liftSlope.SeatsAmount, liftSlope.LiftSlopeingTime, liftSlope.QueueTime);

//            // act & assert
//            await Assert.ThrowsAsync<LiftSlopeAddException>(async () => await sut.AddLiftSlopeAsync(liftSlope.LiftSlopeID, liftSlope.LiftSlopeName, liftSlope.IsOpen, liftSlope.SeatsAmount, liftSlope.LiftSlopeingTime, liftSlope.QueueTime));

//            //assert
//            Assert.Single(await sut.GetLiftSlopesSlopesAsync());
//        }

//        [AllureXunit]
//        [AutoMoqData]
//        public async void TestGetLiftSlopesSlopesAsyncEmpty()
//        {
//            // arrange

//            // act
//            var liftSlopeSlopes = await sut.GetLiftSlopesSlopesAsync();

//            //assert
//            Assert.Empty(liftSlopeSlopes);
//        }

//        [AllureXunitTheory]
//        [AutoMoqData]
//        public async void TestGetLiftSlopeByIdAsyncOk(LiftSlope liftSlope)
//        {
//            // arrange
//            await sut.AddLiftSlopeAsync(liftSlope.LiftSlopeID, liftSlope.LiftSlopeName, liftSlope.IsOpen, liftSlope.SeatsAmount, liftSlope.LiftSlopeingTime, liftSlope.QueueTime);

//            // act
//            var liftSlopeFromRepository = await sut.GetLiftSlopeByIdAsync(liftSlope.LiftSlopeID);

//            //assert
//            Assert.Equal(liftSlope, liftSlopeFromRepository);
//        }

//        [AllureXunitTheory]
//        [AutoMoqData]
//        public async void TestGetLiftSlopeByNameAsyncFailsWhenNameNotExists(string liftSlopeName)
//        {
//            // arrange

//            // act & assert
//            await Assert.ThrowsAsync<LiftSlopeNotFoundException>(async () => await sut.GetLiftSlopeByNameAsync(liftSlopeName));
//        }

//        [AllureXunitTheory]
//        [AutoMoqData]
//        public async void TestUpdateLiftSlopeByIDAsyncOk(LiftSlope liftSlope)
//        {
//            // arrange
//            LiftSlope newLiftSlope = new(liftSlope.LiftSlopeID, liftSlope.LiftSlopeName + "smth", !liftSlope.IsOpen, liftSlope.SeatsAmount, liftSlope.LiftSlopeingTime, liftSlope.QueueTime);
//            await sut.AddLiftSlopeAsync(liftSlope.LiftSlopeID, liftSlope.LiftSlopeName, liftSlope.IsOpen, liftSlope.SeatsAmount, liftSlope.LiftSlopeingTime, liftSlope.QueueTime);

//            // act
//            await sut.UpdateLiftSlopeByIDAsync(liftSlope.LiftSlopeID, newLiftSlope.LiftSlopeName, newLiftSlope.IsOpen, liftSlope.SeatsAmount, liftSlope.LiftSlopeingTime);

//            //assert
//            Assert.True(newLiftSlope.EqualWithoutConnectedSlopesAndQueueTime(await sut.GetLiftSlopeByIdAsync(liftSlope.LiftSlopeID)));
//        }

//        [AllureXunitTheory]
//        [AutoMoqData]
//        public async void TestDeleteLiftSlopeByIDAsyncFailsWhenIDNotExists(uint liftSlopeID)
//        {
//            // arrange

//            // act & assert
//            await Assert.ThrowsAsync<LiftSlopeDeleteException>(async () => await sut.DeleteLiftSlopeByIDAsync(liftSlopeID));
//        }
//    }
//}


