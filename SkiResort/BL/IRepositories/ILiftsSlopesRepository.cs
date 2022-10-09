using BL.Models;

namespace BL.IRepositories
{
    public interface ILiftsSlopesRepository
    {
        Task<List<LiftSlope>> GetLiftsSlopesAsync(uint offset = 0, uint limit = 0);
        Task<LiftSlope> GetLiftSlopeByIdAsync(uint recordID);
        Task<List<Lift>> GetLiftsBySlopeIdAsync(uint slopeID);
        Task<List<Slope>> GetSlopesByLiftIdAsync(uint liftID);
        Task AddLiftSlopeAsync(uint recordID, uint liftID, uint slopeID);
        Task<uint> AddLiftSlopeAutoIncrementAsync(uint liftID, uint slopeID);
        Task UpdateLiftSlopesByIDAsync(uint recordID, uint newLiftID, uint newSlopeID);
        Task DeleteLiftSlopesByIDAsync(uint recordID); 
        Task DeleteLiftSlopesByIDsAsync(uint liftID, uint slopeID); 
    }
}
