using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;
using ProGaudi.Tarantool.Client.Model.Enums;
using ProGaudi.Tarantool.Client.Model.UpdateOperations;

using BL;
using BL.Models;
using BL.IRepositories;
using AccessToDB.Converters;
using AccessToDB.Exceptions.CardExceptions;

namespace AccessToDB.RepositoriesTarantool
{
    public class TarantoolCardsRepository : ICardsRepository
    {
        private ISpace _space;
        private IIndex _indexPrimary;
        private IBox _box;

        public TarantoolCardsRepository(TarantoolContext context)
        {
            _box = context.box;
            _space = context.cardsSpace;
            _indexPrimary = context.cardsIndexPrimary;
        }

        public async Task<List<Card>> GetCardsAsync(uint offset = 0u, uint limit = 0)
        {
            var data = await _indexPrimary.Select<ValueTuple<uint>, CardDB>
                (ValueTuple.Create(0u), new SelectOptions { Iterator = Iterator.Ge });

            List<Card> result = new();

            for (uint i = offset; i < (uint)data.Data.Length && (i < limit || limit == 0); i++)
            {
                result.Add(CardConverter.DBToBL(data.Data[i]));
            }

            return result;
        }

        public async Task<Card> GetCardByIdAsync(uint CardID)
        {
            var data = await _indexPrimary.Select<ValueTuple<uint>, CardDB>
                (ValueTuple.Create(CardID));

            if (data.Data.Length != 1)
            {
                throw new CardNotFoundException(CardID);
            }

            return CardConverter.DBToBL(data.Data[0]);
        }

        public async Task AddCardAsync(uint cardID, DateTimeOffset activationTime, string type)
        {
            try
            {
                await _space.Insert(new CardDB(cardID, (uint) activationTime.ToUnixTimeSeconds(), type));
            }
            catch (Exception ex)
            {
                throw new CardAddException(cardID, activationTime, type);
            }
        }
        public async Task<uint> AddCardAutoIncrementAsync(DateTimeOffset activationTime, string type)
        {
            try
            {
                var result = await _box.Call_1_6<CardDBNoIndex, CardDB>("auto_increment_cards", (new CardDBNoIndex((uint) activationTime.ToUnixTimeSeconds(), type)));
                return CardConverter.DBToBL(result.Data[0]).CardID;
            }
            catch (Exception ex)
            {
                throw new CardAddAutoIncrementException(activationTime, type);
            }
        }
        public async Task UpdateCardByIDAsync(uint cardID, DateTimeOffset newActivationTime, string newType)
        {
            var response = await _space.Update<ValueTuple<uint>, CardDB>(
                ValueTuple.Create(cardID), new UpdateOperation[] {
                    UpdateOperation.CreateAssign<uint>(1, (uint) newActivationTime.ToUnixTimeSeconds()),
                    UpdateOperation.CreateAssign<string>(2, newType),
                });

            if (response.Data.Length != 1)
            {
                throw new CardUpdateException(cardID, newActivationTime, newType);
            }
        }

        public async Task DeleteCarByIDdAsync(uint cardID)
        {
            var response = await _indexPrimary.Delete<ValueTuple<uint>, CardDB>
                (ValueTuple.Create(cardID));

            if (response.Data.Length != 1)
            {
                throw new CardDeleteException(cardID);
            }

        }
    }
}
