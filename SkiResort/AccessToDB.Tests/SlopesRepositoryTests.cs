using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

using ProGaudi.Tarantool.Client;

using BL.Models;
using BL.IRepositories;


using AccessToDB.RepositoriesTarantool;
using AccessToDB.Exceptions.SlopeExceptions;
using System;
using Allure.Xunit.Attributes;
using AccessToDB.Tests.ArrangeHelpers;
using Moq;
using AutoFixture.Xunit2;


//https://xunit.net/docs/shared-context
//Constructor and Dispose
// When to use: when you want a clean test context for every test
// (sharing the setup and cleanup code, without sharing the object instance).

namespace AccessToDB.Tests
{
    [AllureSuite("SlopesRepositorySuite")]
    public class SlopesRepositoryTests: IDisposable
    {
        private ISlopesRepository sut { get { return _slopesRepository; } }
        private ISlopesRepository _slopesRepository;
        private TarantoolContext _tarantoolContext;

        public SlopesRepositoryTests()
        {
            _tarantoolContext = new TarantoolContext("ski_admin:Tty454r293300@localhost:3301");
            _slopesRepository = new TarantoolSlopesRepository(_tarantoolContext);
            CleanSlopesTable().GetAwaiter().GetResult();
        }
        public void Dispose()
        {
            CleanSlopesTable().GetAwaiter().GetResult();
            _tarantoolContext.Dispose();
        }

        private async Task CleanSlopesTable()
        {
            foreach (var slope in await _slopesRepository.GetSlopesAsync())
            {
                await _slopesRepository.DeleteSlopeByIDAsync(slope.SlopeID);
            }
        }

        [AllureXunit]
        [AutoMoqData]
        public async void TestAddSlopeAutoIncrementAsyncOk()
        {
            // arrange
            Slope slope = new(1, "A0", true, 1);

            // act
            await sut.AddSlopeAutoIncrementAsync(slope.SlopeName, slope.IsOpen, slope.DifficultyLevel);

            //assert
            Assert.Equal(slope, await sut.GetSlopeByNameAsync(slope.SlopeName));
            Assert.Single(await sut.GetSlopesAsync());
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAddSlopeAsyncFailsWhenSlopeIdRepeats(Slope slope)
        {
            // arrange
            await sut.AddSlopeAsync(slope.SlopeID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel);

            // act & assert
            await Assert.ThrowsAsync<SlopeAddException>(async () => await sut.AddSlopeAsync(slope.SlopeID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel));

            //assert
            Assert.Single(await sut.GetSlopesAsync());
        }

        [AllureXunit]
        [AutoMoqData]
        public async void TestGetSlopesAsyncEmpty()
        {
            // arrange

            // act
            var slopes = await sut.GetSlopesAsync();

            //assert
            Assert.Empty(slopes);
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestGetSlopeByIdAsyncOk(Slope slope)
        {
            // arrange
            await sut.AddSlopeAsync(slope.SlopeID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel);

            // act
            var slopeFromRepository = await sut.GetSlopeByIdAsync(slope.SlopeID);

            //assert
            Assert.Equal(slope, slopeFromRepository);
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestGetSlopeByNameAsyncFailsWhenNameNotExists(string slopeName)
        {
            // arrange

            // act & assert
            await Assert.ThrowsAsync<SlopeNotFoundException>(async () => await sut.GetSlopeByNameAsync(slopeName));
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestUpdateSlopeByIDAsyncOk(Slope slope)
        {
            // arrange
            Slope newSlope = new(slope.SlopeID, slope.SlopeName + "smth", !slope.IsOpen, slope.DifficultyLevel);
            await sut.AddSlopeAsync(slope.SlopeID, slope.SlopeName, slope.IsOpen, slope.DifficultyLevel);

            // act
            await sut.UpdateSlopeByIDAsync(slope.SlopeID, newSlope.SlopeName, newSlope.IsOpen, newSlope.DifficultyLevel);
            
            //assert
            Assert.Equal(newSlope, await sut.GetSlopeByIdAsync(slope.SlopeID));
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestDeleteSlopeByIDAsyncFailsWhenIDNotExists(uint slopeID)
        {
            // arrange

            // act & assert
            await Assert.ThrowsAsync<SlopeNotFoundException>(async () => await sut.DeleteSlopeByIDAsync(slopeID));
        }
    }
}


