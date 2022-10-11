using System.Threading.Tasks;
using Xunit;
using BL.Models;
using BL.IRepositories;
using AccessToDB.RepositoriesTarantool;
using AccessToDB.Exceptions.LiftSlopeExceptions;
using System;
using Allure.Xunit.Attributes;
using AccessToDB.Tests.ArrangeHelpers;
using Moq;
using AutoFixture.Xunit2;
using AccessToDB.Tests.FakeRepositories;
using System.Collections.Generic;

// лондонский подход -- репозитории ILiftsRepository и ISlopesRepository -- Fake-и
//https://xunit.net/docs/shared-context
//Constructor and Dispose
// When to use: when you want a clean test context for every test
// (sharing the setup and cleanup code, without sharing the object instance).

namespace AccessToDB.Tests
{
    [AllureSuite("LiftsSlopesRepositorySuite")]
    public class LiftsSlopesRepositoryTests : IDisposable
    {
        private ILiftsSlopesRepository sut { get { return _liftsSlopesRepository; } }
        private ILiftsSlopesRepository _liftsSlopesRepository;
        private ILiftsRepository _liftsRepository;
        private ISlopesRepository _slopesRepository;
        private TarantoolContext _tarantoolContext;

        public LiftsSlopesRepositoryTests()
        {
            _tarantoolContext = new TarantoolContext("ski_admin:Tty454r293300@localhost:3301");
            _liftsRepository = new FakeLiftsRepository();
            _slopesRepository = new FakeSlopesRepository();
            _liftsSlopesRepository = new TarantoolLiftsSlopesRepository(_tarantoolContext, _liftsRepository, _slopesRepository);
            CleanLiftsSlopesTable().GetAwaiter().GetResult();
        }
        public void Dispose()
        {
            CleanLiftsSlopesTable().GetAwaiter().GetResult();
            _tarantoolContext.Dispose();
        }

        private async Task CleanLiftsSlopesTable()
        {
            foreach (var liftSlope in await _liftsSlopesRepository.GetLiftsSlopesAsync())
            {
                await _liftsSlopesRepository.DeleteLiftSlopesByIDAsync(liftSlope.RecordID);
            }
        }

        [AllureXunit]
        [AutoMoqData]
        public async void TestAddLiftSlopeAutoIncrementAsyncOk()
        {
            // arrange
            LiftSlope liftSlope = new(1, 1, 1);

            // act
            await sut.AddLiftSlopeAutoIncrementAsync(liftSlope.LiftID, liftSlope.SlopeID);

            //assert
            Assert.Equal(liftSlope, await sut.GetLiftSlopeByIdAsync(liftSlope.RecordID));
            Assert.Single(await sut.GetLiftsSlopesAsync());
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAddLiftSlopeAsyncFailsWhenLiftSlopeIdRepeats(LiftSlope liftSlope)
        {
            // arrange
            await sut.AddLiftSlopeAsync(liftSlope.RecordID, liftSlope.LiftID, liftSlope.SlopeID);

            // act & assert
            await Assert.ThrowsAsync<LiftSlopeAddException>(async () => await sut.AddLiftSlopeAsync(liftSlope.RecordID, liftSlope.LiftID, liftSlope.SlopeID));

            //assert
            Assert.Single(await sut.GetLiftsSlopesAsync());
        }

        [AllureXunit]
        [AutoMoqData]
        public async void TestGetLiftSlopesAsyncEmpty()
        {
            // arrange

            // act
            var liftSlopeSlopes = await sut.GetLiftsSlopesAsync();

            //assert
            Assert.Empty(liftSlopeSlopes);
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestGetLiftSlopeByIdAsyncOk(LiftSlope liftSlope)
        {
            // arrange
            await sut.AddLiftSlopeAsync(liftSlope.RecordID, liftSlope.LiftID, liftSlope.SlopeID);

            // act
            var liftSlopeFromRepository = await sut.GetLiftSlopeByIdAsync(liftSlope.RecordID);

            //assert
            Assert.Equal(liftSlope, liftSlopeFromRepository);
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestUpdateLiftSlopeByIDAsyncOk(LiftSlope liftSlope)
        {
            // arrange
            LiftSlope newLiftSlope = new(liftSlope.RecordID, liftSlope.LiftID + 1, liftSlope.SlopeID + 3);
            await sut.AddLiftSlopeAsync(liftSlope.RecordID, liftSlope.LiftID, liftSlope.SlopeID);

            // act
            await sut.UpdateLiftSlopesByIDAsync(liftSlope.RecordID, newLiftSlope.LiftID, newLiftSlope.SlopeID);

            //assert
            Assert.Equal(newLiftSlope, (await sut.GetLiftSlopeByIdAsync(liftSlope.RecordID)));
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestDeleteLiftSlopeByIDAsyncFailsWhenIDNotExists(uint liftSlopeID)
        {
            // arrange

            // act & assert
            await Assert.ThrowsAsync<LiftSlopeDeleteException>(async () => await sut.DeleteLiftSlopesByIDAsync(liftSlopeID));
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestGetSlopesByLiftIdAsyncFailsWhenSlopeNotExists(LiftSlope liftSlope)
        {
            // arrange
            await sut.AddLiftSlopeAsync(liftSlope.RecordID, liftSlope.LiftID, liftSlope.SlopeID);

            // act & assert
            await Assert.ThrowsAsync<Exception>(async () => await sut.GetSlopesByLiftIdAsync(liftSlope.LiftID));
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestGetLiftsBySlopeIdAsyncOk(Slope slope1, Slope slope2, Lift lift1, Lift lift2)
        {
            // arrange
            uint liftID1 = await _liftsRepository.AddLiftAutoIncrementAsync(lift1.LiftName, lift1.IsOpen, lift1.SeatsAmount, lift1.LiftingTime);
            uint liftID2 = await _liftsRepository.AddLiftAutoIncrementAsync(lift2.LiftName, lift2.IsOpen, lift2.SeatsAmount, lift2.LiftingTime);
            uint slopeID1 = await _slopesRepository.AddSlopeAutoIncrementAsync(slope1.SlopeName, slope1.IsOpen, slope1.DifficultyLevel);
            uint slopeID2 = await _slopesRepository.AddSlopeAutoIncrementAsync(slope2.SlopeName, slope2.IsOpen, slope2.DifficultyLevel);
            await sut.AddLiftSlopeAutoIncrementAsync(liftID1, slopeID1);
            await sut.AddLiftSlopeAutoIncrementAsync(liftID2, slopeID1);
            await sut.AddLiftSlopeAutoIncrementAsync(liftID1, slopeID2);

            // act
            List<Lift> lifts = await sut.GetLiftsBySlopeIdAsync(slopeID1);

            // assert
            Assert.Equal(2, lifts.Count);
            Assert.Equal(liftID1, lifts[0].LiftID);
            Assert.Equal(liftID2, lifts[1].LiftID);
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestDeleteLiftSlopesByIDsAsyncOk(LiftSlope liftSlope)
        {
            // arrange
            await sut.AddLiftSlopeAutoIncrementAsync(liftSlope.LiftID, liftSlope.SlopeID);

            // act
            await sut.DeleteLiftSlopesByIDsAsync(liftSlope.LiftID, liftSlope.SlopeID);

            // assert
            Assert.Empty(await sut.GetLiftsSlopesAsync());
        }
    }
}


