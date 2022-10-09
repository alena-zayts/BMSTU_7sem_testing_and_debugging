using BL.Models;

namespace BL.IRepositories
{
    public interface ICardReadingsRepository
    {
        Task<CardReading> GetCardReadingByIDAsync(uint recordID);
        Task<List<CardReading>> GetCardReadingsAsync(uint offset = 0, uint limit = 0);
        Task<uint> CountForLiftIdFromDateAsync(uint liftID, DateTimeOffset dateFrom, DateTimeOffset dateTo);

        Task AddCardReadingAsync(uint recordID, uint turnstileID, uint cardID, DateTimeOffset readingTime);
        Task DeleteCardReadingAsync(uint recordID);
        Task UpdateCardReadingByIDAsync(uint recordID, uint newTurnstileID, uint newCardID, DateTimeOffset newReadingTime);
        Task<uint> AddCardReadingAutoIncrementAsync(uint turnstileID, uint cardID, DateTimeOffset readingTime);
        Task<uint> UpdateQueueTime(uint liftID, DateTimeOffset dateFrom, DateTimeOffset dateTo);
    }
}
