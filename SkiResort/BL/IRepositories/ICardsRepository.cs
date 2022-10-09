using BL.Models;

namespace BL.IRepositories
{
    public interface ICardsRepository
    {
        Task<List<Card>> GetCardsAsync(uint offset = 0, uint limit = 0);
        Task<Card> GetCardByIdAsync(uint cardID);
        Task AddCardAsync(uint cardID, DateTimeOffset activationTime, string type);
        Task<uint> AddCardAutoIncrementAsync(DateTimeOffset activationTime, string type);
        Task UpdateCardByIDAsync(uint cardID, DateTimeOffset newActivationTime, string newType);
        Task DeleteCarByIDdAsync(uint cardID);
    }
}
