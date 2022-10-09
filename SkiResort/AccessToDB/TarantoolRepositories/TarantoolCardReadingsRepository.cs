using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;
using ProGaudi.Tarantool.Client.Model.Enums;
using ProGaudi.Tarantool.Client.Model.UpdateOperations;

using BL;
using BL.Models;
using BL.IRepositories;
using AccessToDB.Converters;
using AccessToDB.Exceptions.CardReadingExceptions;

namespace AccessToDB.RepositoriesTarantool
{
    public class TarantoolCardReadingsRepository : ICardReadingsRepository
    {
        private IBox _box;
        private ISpace _space;
        private IIndex _indexPrimary;
        private IIndex _indexTurnstile;

        public TarantoolCardReadingsRepository(TarantoolContext context)
        {
            _box = context.box;
            _space = context.cardReadingsSpace;
            _indexPrimary = context.cardReadingsIndexPrimary;
            _indexTurnstile = context.cardReadingsIndexTurnstile;
    }

        public async Task<List<CardReading>> GetCardReadingsAsync(uint offset = 0u, uint limit = 0)
        {
            var data = await _indexPrimary.Select<ValueTuple<uint>, CardReadingDB>
                (ValueTuple.Create(0u), new SelectOptions { Iterator = Iterator.Ge });

            List<CardReading> result = new();

            for (uint i = offset; i < (uint)data.Data.Length && (i < limit || limit == 0); i++)
            {
                result.Add(CardReadingConverter.DBToBL(data.Data[i]));
            }

            return result;
        }

        public async Task<uint> CountForLiftIdFromDateAsync(uint LiftID, DateTimeOffset dateFrom, DateTimeOffset dateTo)
        {
            try
            {
                var result = await _box.Call_1_6<ValueTuple<uint, uint, uint>, Int32[]>("count_card_readings", (ValueTuple.Create(LiftID, (uint) dateFrom.ToUnixTimeSeconds(), (uint)dateTo.ToUnixTimeSeconds())));
                return (uint) result.Data[0][0];
            }
            catch (Exception ex)
            {
                throw new CountCardReadingsException();
            }
        }
        public async Task<uint> AddCardReadingAutoIncrementAsync(uint turnstileID, uint cardID, DateTimeOffset readingTime)
        {
            try
            {
                var result = await _box.Call_1_6<CardReadingDBNoIndex, CardReadingDB>("auto_increment_card_readings", (new CardReadingDBNoIndex(turnstileID, cardID, (uint) readingTime.ToUnixTimeSeconds())));
                return CardReadingConverter.DBToBL(result.Data[0]).RecordID;
            }
            catch (Exception ex)
            {
                throw new CardReadingAddAutoIncrementException();
            }
        }


        public async Task AddCardReadingAsync(uint recordID, uint turnstileID, uint cardID, DateTimeOffset readingTime)
        {
            try
            {
                await _space.Insert(new CardReadingDB(recordID, turnstileID, cardID, (uint)readingTime.ToUnixTimeSeconds()));
            }
            catch (Exception ex)
            {
                throw new CardReadingAddException();
            }
        }


        public async Task DeleteCardReadingAsync(uint recordID)
        {
            var response = await _indexPrimary.Delete<ValueTuple<uint>, CardReadingDB>
                (ValueTuple.Create(recordID));

            if (response.Data.Length != 1)
            {
                throw new CardReadingDeleteException();
            }

        }

        public async Task<CardReading> GetCardReadingByIDAsync(uint recordID)
        {
            var data = await _indexPrimary.Select<ValueTuple<uint>, CardReadingDB>
                (ValueTuple.Create(recordID));

            if (data.Data.Length != 1)
            {
                throw new CardReadingNotFoundException(recordID);
            }

            return CardReadingConverter.DBToBL(data.Data[0]);
        }

        public async Task UpdateCardReadingByIDAsync(uint recordID, uint newTurnstileID, uint newCardID, DateTimeOffset newReadingTime)
        { 
            var response = await _space.Update<ValueTuple<uint>, CardReadingDB>(
                ValueTuple.Create(recordID), new UpdateOperation[] {
                    UpdateOperation.CreateAssign<uint>(1, newTurnstileID),
                    UpdateOperation.CreateAssign<uint>(2, newCardID),
                    UpdateOperation.CreateAssign<uint>(3, (uint) newReadingTime.ToUnixTimeSeconds()),
                    
                });

            if (response.Data.Length != 1)
            {
                throw new CardReadingUpdateException(recordID, newTurnstileID, newCardID, newReadingTime);
            }
        }

        public async Task<uint> UpdateQueueTime(uint liftID, DateTimeOffset dateFrom, DateTimeOffset dateTo)
        {
            try
            {
                var result = await _box.Call_1_6<ValueTuple<uint, uint, uint>, Int32[]>("update_queue_time", (ValueTuple.Create(liftID, (uint)dateFrom.ToUnixTimeSeconds(), (uint)dateTo.ToUnixTimeSeconds())));
                return (uint)result.Data[0][0];
            }
            catch (Exception ex)
            {
                throw new CountCardReadingsException();
            }
        }
    }
}
