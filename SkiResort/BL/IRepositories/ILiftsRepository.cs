using BL.Models;

namespace BL.IRepositories
{
    public interface ILiftsRepository
    {
        Task<List<Lift>> GetLiftsAsync(uint offset = 0, uint limit = 0);
        Task<Lift> GetLiftByIdAsync(uint liftID);
        Task<Lift> GetLiftByNameAsync(string name);
        Task AddLiftAsync(uint liftID, string liftName, bool isOpen, uint seatsAmount, uint liftingTime, uint queueTime);
        Task<uint> AddLiftAutoIncrementAsync(string liftName, bool isOpen, uint seatsAmount, uint liftingTime);
        Task UpdateLiftByIDAsync(uint liftID, string liftName, bool newIsOpen, uint newSeatsAmount, uint newLiftingTime);
        Task DeleteLiftByIDAsync(uint liftID); 
    }
}
