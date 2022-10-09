using BL.Models;


namespace BL.IRepositories
{
    public interface ITurnstilesRepository
    {
        Task<List<Turnstile>> GetTurnstilesAsync(uint offset = 0, uint limit = 0);
        Task<Turnstile> GetTurnstileByIdAsync(uint turnstileID);
        Task AddTurnstileAsync(uint turnstileID, uint liftID, bool isOpen);
        Task<uint> AddTurnstileAutoIncrementAsync(uint liftID, bool isOpen);
        Task UpdateTurnstileByIDAsync(uint turnstileID, uint newLiftID, bool newIsOpen);
        Task DeleteTurnstileByIDAsync(uint turnstileID);
        Task<List<Turnstile>> GetTurnstilesByLiftIdAsync(uint liftID);
    }
}
