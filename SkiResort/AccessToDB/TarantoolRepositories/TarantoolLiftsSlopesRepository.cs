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
using AccessToDB.Exceptions.SlopeExceptions;
using AccessToDB.Exceptions.LiftSlopeExceptions;

namespace AccessToDB.RepositoriesTarantool
{
    public class TarantoolLiftsSlopesRepository : ILiftsSlopesRepository
    {
        private IIndex _indexPrimary;
        private IIndex _indexLiftID;
        private IIndex _indexSlopeID;
        private ISpace _space;
        private ISlopesRepository _slopesRepository;
        private ILiftsRepository _liftsRepository;
        private IBox _box;

        public TarantoolLiftsSlopesRepository(TarantoolContext context)
        {
            _space = context.liftsSlopesSpace;
            _indexPrimary = context.liftsSlopesIndexPrimary;
            _indexLiftID = context.liftsSlopesIndexLiftID;
            _indexSlopeID = context.liftsSlopesIndexSlopeID;
            _liftsRepository = new TarantoolLiftsRepository(context);
            _slopesRepository = new TarantoolSlopesRepository(context);
            _box = context.box;
        }

        public async Task<List<LiftSlope>> GetLiftsSlopesAsync(uint offset = 0u, uint limit = 0)
        {
            var data = await _indexPrimary.Select<ValueTuple<uint>, LiftSlopeDB>
                (ValueTuple.Create(0u), new SelectOptions { Iterator = Iterator.Ge });

            List<LiftSlope> result = new();

            for (uint i = offset; i < (uint)data.Data.Length && (i < limit || limit == 0); i++)
            {
                result.Add(LiftSlopeConverter.DBToBL(data.Data[i]));
            }

            return result;
        }

        public async Task<LiftSlope> GetLiftSlopeByIdAsync(uint RecordID)
        {
            var data = await _indexPrimary.Select<ValueTuple<uint>, LiftSlopeDB>
                (ValueTuple.Create(RecordID));

            if (data.Data.Length != 1)
            {
                throw new LiftSlopeNotFoundException();
            }

            return LiftSlopeConverter.DBToBL(data.Data[0]);
        }
        private async Task<List<uint>> GetLiftIdsBySlopeId(uint SlopeID)
        {
            List<uint> result = new List<uint>();
            var data = await _indexSlopeID.Select<ValueTuple<uint>, LiftSlopeDB>
                (ValueTuple.Create(SlopeID));

            foreach (var item in data.Data)
            {
                LiftSlope lift_slope = LiftSlopeConverter.DBToBL(item);
                result.Add(lift_slope.LiftID);
            }

            return result;
        }
        public async Task<List<Lift>> GetLiftsBySlopeIdAsync(uint SlopeID)
        {
            List<Lift> result = new List<Lift>();
            List<uint> lift_ids = await GetLiftIdsBySlopeId(SlopeID);

            foreach (var LiftID in lift_ids)
            {
                try
                {
                    var lift = await _liftsRepository.GetLiftByIdAsync(LiftID);
                    result.Add(lift);

                }
                catch (LiftNotFoundException)
                {
                    throw new LiftSlopeLiftNotFoundException();
                }
            }
            return result;
        }


        private async Task<List<uint>> GetSlopeIdsByLiftId(uint LiftID)
        {
            List<uint> result = new List<uint>();
            var data = await _indexLiftID.Select<ValueTuple<uint>, LiftSlopeDB>
                (ValueTuple.Create(LiftID));

            foreach (var item in data.Data)
            {
                LiftSlope lift_slope = LiftSlopeConverter.DBToBL(item);
                result.Add(lift_slope.SlopeID);
            }

            return result;
        }
        public async Task<List<Slope>> GetSlopesByLiftIdAsync(uint LiftID)
        {
            List<Slope> result = new();
            List<uint> slope_ids = await GetSlopeIdsByLiftId(LiftID);

            foreach (var SlopeID in slope_ids)
            {
                try
                {
                    var slope = await _slopesRepository.GetSlopeByIdAsync(SlopeID);
                    result.Add(slope);

                }
                catch (SlopeNotFoundException)
                {
                    throw new LiftSlopeSlopeNotFoundException();
                }
            }
            return result;
        }


        public async Task AddLiftSlopeAsync(uint recordID, uint liftID, uint slopeID)
        {
            try
            {
                await _space.Insert(new LiftSlopeDB(recordID, liftID, slopeID));
            }
            catch (Exception ex)
            {
                throw new LiftSlopeAddException();
            }
        }

        public async Task<uint> AddLiftSlopeAutoIncrementAsync(uint liftID, uint slopeID)
        {
            try
            {
                var result = await _box.Call_1_6<LiftSlopeDBNoIndex, LiftSlopeDB>("auto_increment_lifts_slopes", (new LiftSlopeDBNoIndex(liftID, slopeID)));
                return LiftSlopeConverter.DBToBL(result.Data[0]).RecordID;
            }
            catch (Exception ex)
            {
                throw new LiftSlopeAddAutoIncrementException();
            }
        }

        public async Task UpdateLiftSlopesByIDAsync(uint recordID, uint newLiftID, uint newSlopeID)
        {
            var response = await _space.Update<ValueTuple<uint>, LiftSlopeDB>(
                ValueTuple.Create(recordID), new UpdateOperation[] {
                    UpdateOperation.CreateAssign<uint>(1, newLiftID),
                    UpdateOperation.CreateAssign<uint>(2, newSlopeID),
                });

            if (response.Data.Length != 1)
            {
                throw new LiftSlopeUpdateException();
            }
        }

        public async Task DeleteLiftSlopesByIDAsync(uint recordID)
        {
            var response = await _indexPrimary.Delete<ValueTuple<uint>, LiftSlopeDB>
                (ValueTuple.Create(recordID));

            if (response.Data.Length != 1)
            {
                throw new LiftSlopeDeleteException();
            }

        }

        public async Task DeleteLiftSlopesByIDsAsync(uint liftID, uint slopeID)
        {
            List<LiftSlope> liftSlopes = await GetLiftsSlopesAsync();
            foreach (LiftSlope liftSlope in liftSlopes)
            {
                if (liftSlope.LiftID == liftID && liftSlope.SlopeID == slopeID)
                {
                    await DeleteLiftSlopesByIDAsync(liftSlope.RecordID);
                    return;
                }
            }
            throw new LiftSlopeNotFoundException();
        }
    }
}

