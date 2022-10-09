using BL.Models;

namespace BL.Exceptions.MessageExceptions
{
    public class MessageCheckingException : Exception
    {
        public Message? MessageModel { get; }
        public MessageCheckingException() : base() { }
        public MessageCheckingException(string? message) : base(message) { }
        public MessageCheckingException(string? message, Exception? innerException) : base(message, innerException) { }

        public MessageCheckingException(string? message, Message? messageModel) : base(message)
        {
            this.MessageModel = messageModel;
        }

    }
}
