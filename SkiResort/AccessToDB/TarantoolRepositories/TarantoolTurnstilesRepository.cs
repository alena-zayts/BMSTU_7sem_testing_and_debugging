using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;
using ProGaudi.Tarantool.Client.Model.Enums;
using ProGaudi.Tarantool.Client.Model.UpdateOperations;

using BL;
using BL.Models;
using BL.IRepositories;
using AccessToDB.Converters;
using AccessToDB.Exceptions.TurnstileExceptions;

namespace AccessToDB.RepositoriesTarantool
{
    public class TarantoolTurnstilesRepository : ITurnstilesRepository
    {
        private IIndex _indexPrimary;
        private IIndex _indexLiftID;
        private ISpace _space;
        private IBox _box;

        public TarantoolTurnstilesRepository(TarantoolContext context)
        {
            _space = context.turnstilesSpace;
            _indexPrimary = context.turnstilesIndexPrimary;
            _indexLiftID = context.turnstilesIndexLiftID;
            _box = context.box;
        }

        public async Task<List<Turnstile>> GetTurnstilesAsync(uint offset = 0u, uint limit = 0)
        {
            var data = await _indexPrimary.Select<ValueTuple<uint>, TurnstileDB>
                (ValueTuple.Create(0u), new SelectOptions { Iterator = Iterator.Ge });

            List<Turnstile> result = new();

            for (uint i = offset; i < (uint)data.Data.Length && (i < limit || limit ==0); i++)
            {
                result.Add(TurnstileConverter.DBToBL(data.Data[i]));
            }

            return result;
        }

        public async Task<List<Turnstile>> GetTurnstilesByLiftIdAsync(uint LiftID)
        {
            var data = await _indexLiftID.Select<ValueTuple<uint>, TurnstileDB>
                (ValueTuple.Create(LiftID));

            List<Turnstile> result = new();

            foreach (var item in data.Data)
            {
                Turnstile turnstile = TurnstileConverter.DBToBL(item);
                result.Add(turnstile);
            }

            return result;
        }

        public async Task<Turnstile> GetTurnstileByIdAsync(uint TurnstileID)
        {
            var data = await _indexPrimary.Select<ValueTuple<uint>, TurnstileDB>
                (ValueTuple.Create(TurnstileID));

            if (data.Data.Length != 1)
            {
                throw new TurnstileNotFoundException();
            }

            return TurnstileConverter.DBToBL(data.Data[0]);
        }

        public async Task AddTurnstileAsync(uint turnstileID, uint liftID, bool isOpen)
        {
            try
            {
                await _space.Insert(new TurnstileDB(turnstileID, liftID, isOpen));
            }
            catch (Exception ex)
            {
                throw new TurnstileAddException();
            }
        }

        public async Task<uint> AddTurnstileAutoIncrementAsync(uint liftID, bool isOpen)
        {
            try
            {
                var result = await _box.Call_1_6<TurnstileDBNoIndex, TurnstileDB>("auto_increment_turnstiles", (new TurnstileDBNoIndex(liftID, isOpen)));
                return TurnstileConverter.DBToBL(result.Data[0]).TurnstileID;
            }
            catch (Exception ex)
            {
                throw new TurnstileAddAutoIncrementException();
            }
        }

        public async Task UpdateTurnstileByIDAsync(uint turnstileID, uint newLiftID, bool newIsOpen)
        {
            var response = await _space.Update<ValueTuple<uint>, TurnstileDB>(
                ValueTuple.Create(turnstileID), new UpdateOperation[] {
                    UpdateOperation.CreateAssign<uint>(1, newLiftID),
                    UpdateOperation.CreateAssign<bool>(2, newIsOpen),
                });

            if (response.Data.Length != 1)
            {
                throw new TurnstileUpdateException();
            }
        }

        public async Task DeleteTurnstileByIDAsync(uint turnstileID)
        {
            var response = await _indexPrimary.Delete<ValueTuple<uint>, TurnstileDB>
                (ValueTuple.Create(turnstileID));

            if (response.Data.Length != 1)
            {
                throw new TurnstileDeleteException();
            }

        }
    }
}

