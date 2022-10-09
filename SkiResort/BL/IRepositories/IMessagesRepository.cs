using BL.Models;


namespace BL.IRepositories
{
    public interface IMessagesRepository
    {
        Task<List<Message>> GetMessagesAsync(uint offset = 0, uint limit = 0);
        Task<Message> GetMessageByIdAsync(uint messageID);
        Task AddMessageAsync(uint messageID, uint senderID, uint checkedByID, string text);
        Task<uint> AddMessageAutoIncrementAsync(uint senderID, uint checkedByID, string text);
        Task UpdateMessageByIDAsync(uint messageID, uint newSenderID, uint newCheckedByID, string newText); 
        Task DeleteMessageByIDAsync(uint messageID);
        Task<List<Message>> GetMessagesBySenderIdAsync(uint senderID);
        Task<List<Message>> GetMessagesByCheckerIdAsync(uint checkedByID);
    }
}
