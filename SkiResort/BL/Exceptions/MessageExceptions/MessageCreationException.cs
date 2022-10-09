using BL.Models;

namespace BL.Exceptions.MessageExceptions
{
    public class MessageCreationException : Exception
    {
        public Message? MessageModel { get; }
        public MessageCreationException() : base() { }
        public MessageCreationException(string? message) : base(message) { }
        public MessageCreationException(string? message, Exception? innerException) : base(message, innerException) { }

        public MessageCreationException(string? message, Message? messageModel) : base(message)
        {
            this.MessageModel = messageModel;
        }

    }
}
