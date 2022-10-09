using BL.Models;

namespace BL.IRepositories
{
    public interface ISlopesRepository
    {
        Task<List<Slope>> GetSlopesAsync(uint offset = 0, uint limit = 0);
        Task<Slope> GetSlopeByIdAsync(uint SlopeID);
        Task<Slope> GetSlopeByNameAsync(string name);
        Task AddSlopeAsync(uint slopeID, string slopeName, bool isOpen, uint difficultyLevel);
        Task<uint> AddSlopeAutoIncrementAsync(string slopeName, bool isOpen, uint difficultyLevel);
        Task UpdateSlopeByIDAsync(uint slopeID, string newSlopeName, bool newIsOpen, uint newDifficultyLevel);
        Task DeleteSlopeByIDAsync(uint slopeID); 
    }
}
