using BL.Models;


namespace BL.IRepositories
{
    public interface IUsersRepository
    {
        Task<List<User>> GetUsersAsync(uint offset = 0, uint limit = 0);
        Task<User> GetUserByIdAsync(uint userID);
        Task<User> GetUserByEmailAsync(string userEmail);
        Task<bool> CheckUserIdExistsAsync(uint userID);
        Task<bool> CheckUserEmailExistsAsync(string userEmail);
        Task AddUserAsync(uint userID, uint cardID, string UserEmail, string password, PermissionsEnum permissions);
        Task<uint> AddUserAutoIncrementAsync( uint cardID, string UserEmail, string password, PermissionsEnum permissions);
        Task UpdateUserByIDAsync(uint userID, uint newCardID, string newUserEmail, string newPassword, PermissionsEnum newPermissions); 
        Task DeleteUserByIDAsync(uint userID); 
    }
}
