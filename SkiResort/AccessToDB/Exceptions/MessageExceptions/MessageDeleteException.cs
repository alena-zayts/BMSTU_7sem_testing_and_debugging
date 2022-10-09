using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.MessageExceptions
{
    public class MessageDeleteException : MessageException
    {
        public MessageDeleteException() : base() { }
        public MessageDeleteException(string? message) : base(message) { }
        public MessageDeleteException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
