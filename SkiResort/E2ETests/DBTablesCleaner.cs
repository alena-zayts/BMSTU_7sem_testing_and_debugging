using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.IRepositories;
using AccessToDB.RepositoriesTarantool;
using AccessToDB;
using Xunit;

namespace E2ETests
{
    public class DBTablesCleaner
    {
        private ILiftsRepository liftsRepository;
        private ISlopesRepository slopesRepository;
        private ILiftsSlopesRepository liftsSlopesRepository;
        private IUsersRepository usersRepository;
        private ITurnstilesRepository turnstilesRepository;

        public DBTablesCleaner(TarantoolContext tarantoolContext)
        {
            this.liftsRepository = new TarantoolLiftsRepository(tarantoolContext);
            this.slopesRepository = new TarantoolSlopesRepository(tarantoolContext);
            this.liftsSlopesRepository = new TarantoolLiftsSlopesRepository(tarantoolContext, liftsRepository, slopesRepository);
            this.usersRepository = new TarantoolUsersRepository(tarantoolContext);
            this.turnstilesRepository = new TarantoolTurnstilesRepository(tarantoolContext);
        }

        public async Task clean()
        {
            await cleanLiftsSlopesTable();
            await cleanTurnstilesTable();
            await cleanSlopesTable();
            await cleanLiftsTable();
            await cleaUsersTable();
            Assert.Empty(await liftsSlopesRepository.GetLiftsSlopesAsync());
            Assert.Empty(await slopesRepository.GetSlopesAsync());
            Assert.Empty(await liftsRepository.GetLiftsAsync());
            Assert.Empty(await usersRepository.GetUsersAsync());
            Assert.Empty(await turnstilesRepository.GetTurnstilesAsync());
        }

        private async Task cleanLiftsSlopesTable()
        {
            var objes = await liftsSlopesRepository.GetLiftsSlopesAsync();
            foreach (var obj in objes)
            {
                await liftsSlopesRepository.DeleteLiftSlopesByIDAsync(obj.RecordID);
            }
        }
        private async Task cleanTurnstilesTable()
        {
            var objes = await turnstilesRepository.GetTurnstilesAsync();
            foreach (var obj in objes)
            {
                await turnstilesRepository.DeleteTurnstileByIDAsync(obj.TurnstileID);
            }
        }

        private async Task cleanLiftsTable()
        {
            var objes = await liftsRepository.GetLiftsAsync();
            foreach (var obj in objes)
            {
                await liftsRepository.DeleteLiftByIDAsync(obj.LiftID);
            }
        }
        private async Task cleanSlopesTable()
        {
            var objes = await slopesRepository.GetSlopesAsync();
            foreach (var obj in objes)
            {
                await slopesRepository.DeleteSlopeByIDAsync(obj.SlopeID);
            }
        }

        private async Task cleaUsersTable()
        {
            var objes = await usersRepository.GetUsersAsync();
            foreach (var obj in objes)
            {
                await usersRepository.DeleteUserByIDAsync(obj.UserID);
            }
        }

    }
}
