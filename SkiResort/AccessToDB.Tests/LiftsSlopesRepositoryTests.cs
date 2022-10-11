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
        private TarantoolContext _tarantoolContext;

        public LiftsSlopesRepositoryTests()
        {
            _tarantoolContext = new TarantoolContext("ski_admin:Tty454r293300@localhost:3301");
            var _liftsRepository = new FakeLiftsRepository();
            var _slopesRepository = new FakeSlopesRepository();
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

        //[AllureXunitTheory]
        //[AutoMoqData]
        //public async void TestGetLiftSlopeByNameAsyncFailsWhenNameNotExists(string liftSlopeName)
        //{
        //    // arrange

        //    // act & assert
        //    await Assert.ThrowsAsync<LiftSlopeNotFoundException>(async () => await sut.GetLiftSlopeByNameAsync(liftSlopeName));
        //}

        //public async Task<List<Slope>> GetSlopesByLiftIdAsync(uint LiftID)
        //{
        //    List<Slope> result = new();
        //    List<uint> slope_ids = await GetSlopeIdsByLiftId(LiftID);

        //    foreach (var SlopeID in slope_ids)
        //    {
        //        try
        //        {
        //            var slope = await _slopesRepository.GetSlopeByIdAsync(SlopeID);
        //            result.Add(slope);

        //        }
        //        catch (SlopeNotFoundException)
        //        {
        //            throw new LiftSlopeSlopeNotFoundException();
        //        }
        //    }
        //    return result;
        //}

        //public async Task<List<Lift>> GetLiftsBySlopeIdAsync(uint SlopeID)
        //{
        //    List<Lift> result = new List<Lift>();
        //    List<uint> lift_ids = await GetLiftIdsBySlopeId(SlopeID);

        //    foreach (var LiftID in lift_ids)
        //    {
        //        try
        //        {
        //            var lift = await _liftsRepository.GetLiftByIdAsync(LiftID);
        //            result.Add(lift);

        //        }
        //        catch (LiftNotFoundException)
        //        {
        //            throw new LiftSlopeLiftNotFoundException();
        //        }
        //    }
        //    return result;
        //}

        //public async Task DeleteLiftSlopesByIDsAsync(uint liftID, uint slopeID)
        //{
        //    List<LiftSlope> liftSlopes = await GetLiftsSlopesAsync();
        //    foreach (LiftSlope liftSlope in liftSlopes)
        //    {
        //        if (liftSlope.LiftID == liftID && liftSlope.SlopeID == slopeID)
        //        {
        //            await DeleteLiftSlopesByIDAsync(liftSlope.RecordID);
        //            return;
        //        }
        //    }
        //    throw new LiftSlopeNotFoundException();
        //}
    }
}


