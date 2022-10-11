using System.Threading.Tasks;
using Xunit;
using BL.Models;
using BL.IRepositories;
using AccessToDB.RepositoriesTarantool;
using AccessToDB.Exceptions.TurnstileExceptions;
using System;
using Allure.Xunit.Attributes;
using AccessToDB.Tests.ArrangeHelpers;
using System.Collections.Generic;

// классический подход
//https://xunit.net/docs/shared-context
//Constructor and Dispose
// When to use: when you want a clean test context for every test
// (sharing the setup and cleanup code, without sharing the object instance).

namespace AccessToDB.Tests
{
    [AllureSuite("TurnstilesRepositorySuite")]
    public class TurnstilesRepositoryTests : IDisposable
    {
        private ITurnstilesRepository sut { get { return _turnstilesRepository; } }
        private ITurnstilesRepository _turnstilesRepository;
        private TarantoolContext _tarantoolContext;

        public TurnstilesRepositoryTests()
        {
            _tarantoolContext = new TarantoolContext("ski_admin:Tty454r293300@localhost:3301");
            _turnstilesRepository = new TarantoolTurnstilesRepository(_tarantoolContext);
            CleanTurnstilesTable().GetAwaiter().GetResult();
        }
        public void Dispose()
        {
            CleanTurnstilesTable().GetAwaiter().GetResult();
            _tarantoolContext.Dispose();
        }

        private async Task CleanTurnstilesTable()
        {
            foreach (var turnstile in await _turnstilesRepository.GetTurnstilesAsync())
            {
                await _turnstilesRepository.DeleteTurnstileByIDAsync(turnstile.TurnstileID);
            }
        }

        [AllureXunit]
        [AutoMoqData]
        public async void TestAddTurnstileAutoIncrementAsyncOk()
        {
            // arrange
            Turnstile turnstile = new(1, 1, true);

            // act
            await sut.AddTurnstileAutoIncrementAsync(turnstile.LiftID, turnstile.IsOpen);

            //assert
            Assert.Equal(turnstile, await sut.GetTurnstileByIdAsync(turnstile.TurnstileID));
            Assert.Single(await sut.GetTurnstilesAsync());
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestAddTurnstileAsyncFailsWhenTurnstileIdRepeats(Turnstile turnstile)
        {
            // arrange
            await sut.AddTurnstileAsync(turnstile.TurnstileID, turnstile.LiftID, turnstile.IsOpen);

            // act & assert
            await Assert.ThrowsAsync<TurnstileAddException>(async () => await sut.AddTurnstileAsync(turnstile.TurnstileID, turnstile.LiftID, turnstile.IsOpen));

            //assert
            Assert.Single(await sut.GetTurnstilesAsync());
        }

        [AllureXunit]
        [AutoMoqData]
        public async void TestGetTurnstilesAsyncEmpty()
        {
            // arrange

            // act
            var turnstiles = await sut.GetTurnstilesAsync();

            //assert
            Assert.Empty(turnstiles);
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestGetTurnstileByIdAsyncOk(Turnstile turnstile)
        {
            // arrange
            await sut.AddTurnstileAsync(turnstile.TurnstileID, turnstile.LiftID, turnstile.IsOpen);

            // act
            var turnstileFromRepository = await sut.GetTurnstileByIdAsync(turnstile.TurnstileID);

            //assert
            Assert.Equal(turnstile, turnstileFromRepository);
        }


        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestUpdateTurnstileByIDAsyncOk(Turnstile turnstile)
        {
            // arrange
            Turnstile newTurnstile = new(turnstile.TurnstileID, turnstile.TurnstileID + 5, !turnstile.IsOpen);
            await sut.AddTurnstileAsync(turnstile.TurnstileID, turnstile.LiftID, turnstile.IsOpen);

            // act
            await sut.UpdateTurnstileByIDAsync(turnstile.TurnstileID, newTurnstile.LiftID, newTurnstile.IsOpen);

            //assert
            Assert.Equal(newTurnstile, await sut.GetTurnstileByIdAsync(turnstile.TurnstileID));
        }

        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestDeleteTurnstileByIDAsyncFailsWhenIDNotExists(uint turnstileID)
        {
            // arrange

            // act & assert
            await Assert.ThrowsAsync<TurnstileDeleteException>(async () => await sut.DeleteTurnstileByIDAsync(turnstileID));
        }


        [AllureXunitTheory]
        [AutoMoqData]
        public async void TestGetTurnstilesByLiftIdAsyncOk(uint liftID)
        {
            // arrange
            List<Turnstile> correctTurnstiles = new List<Turnstile>() { };
            uint n = 3;
            for (uint i = 0; i < n; i++)
            {
                await _turnstilesRepository.AddTurnstileAsync(i, liftID, true);
                await _turnstilesRepository.AddTurnstileAsync(i + n, liftID + 1, true);
                correctTurnstiles.Add(new Turnstile(i, liftID, true));
            }

            // act
            var liftsFromRepository = await sut.GetTurnstilesByLiftIdAsync(liftID);

            //assert
            Assert.Equal(correctTurnstiles, liftsFromRepository);
        }


        //Task<List<Turnstile>> GetTurnstilesByLiftIdAsync(uint liftID);
    }
}


