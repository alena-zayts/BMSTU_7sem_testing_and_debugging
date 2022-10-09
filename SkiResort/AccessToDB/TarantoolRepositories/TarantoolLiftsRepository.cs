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
using AccessToDB.Exceptions.LiftExceptions;

namespace AccessToDB.RepositoriesTarantool
{
    public class TarantoolLiftsRepository : ILiftsRepository
    {
        private IIndex _indexPrimary;
        private IIndex _indexName;
        private ISpace _space;
        private IBox _box;

        public TarantoolLiftsRepository(TarantoolContext context)
        {
            _space = context.liftsSpace;
            _indexPrimary = context.liftsIndexPrimary;
            _indexName = context.liftsIndexName;
            _box = context.box;
        }

        public async Task<List<Lift>> GetLiftsAsync(uint offset = 0u, uint limit = 0)
        {
            var data = await _indexPrimary.Select<ValueTuple<uint>, LiftDB>
                (ValueTuple.Create(0u), new SelectOptions { Iterator = Iterator.Ge });

            List<Lift> result = new();

            for (uint i = offset; i < (uint)data.Data.Length && (i < limit || limit == 0); i++)
            {
                result.Add(LiftConverter.DBToBL(data.Data[i]));
            }

            return result;
        }

        public async Task<Lift> GetLiftByIdAsync(uint LiftID)
        {
            var data = await _indexPrimary.Select<ValueTuple<uint>, LiftDB>
                (ValueTuple.Create(LiftID));

            if (data.Data.Length != 1)
            {
                throw new LiftNotFoundException();
            }

            return LiftConverter.DBToBL(data.Data[0]);
        }

        public async Task<Lift> GetLiftByNameAsync(string name)
        {
            var data = await _indexName.Select<ValueTuple<string>, LiftDB>
                (ValueTuple.Create(name));

            if (data.Data.Length != 1)
            {
                throw new LiftNotFoundException();
            }

            return LiftConverter.DBToBL(data.Data[0]);
        }

        public async Task AddLiftAsync(uint liftID, string liftName, bool isOpen, uint seatsAmount, uint liftingTime, uint queueTime)
        {
            try
            {
                await _space.Insert(new LiftDB(liftID, liftName, isOpen, seatsAmount, liftingTime, queueTime));
            }
            catch (Exception ex)
            {
                throw new LiftAddException();
            }
        }

        public async Task<uint> AddLiftAutoIncrementAsync(string liftName, bool isOpen, uint seatsAmount, uint liftingTime)
        {
            try
            {
                var result = await _box.Call_1_6<LiftDBNoIndex, LiftDB>("auto_increment_lifts", (new LiftDBNoIndex(liftName, isOpen, seatsAmount, liftingTime, 0)));
                return LiftConverter.DBToBL(result.Data[0]).LiftID;
            }
            catch (Exception ex)
            {
                throw new LiftAddAutoIncrementException();
            }
        }
        public async Task UpdateLiftByIDAsync(uint liftID, string liftName, bool newIsOpen, uint newSeatsAmount, uint newLiftingTime)
        {
            var response = await _space.Update<ValueTuple<uint>, LiftDB>(
                ValueTuple.Create(liftID), new UpdateOperation[] {
                    UpdateOperation.CreateAssign<string>(1, liftName),
                    UpdateOperation.CreateAssign<bool>(2, newIsOpen),
                    UpdateOperation.CreateAssign<uint>(3, newSeatsAmount),
                    UpdateOperation.CreateAssign<uint>(4, newLiftingTime),
                    UpdateOperation.CreateAssign<uint>(5, 0),
                });

            if (response.Data.Length != 1)
            {
                throw new LiftUpdateException();
            }
        }

        public async Task DeleteLiftByIDAsync(uint liftID)
        {
            var response = await _indexPrimary.Delete<ValueTuple<uint>, LiftDB>
                (ValueTuple.Create(liftID));

            if (response.Data.Length != 1)
            {
                throw new LiftDeleteException();
            }

        }
    }
}

