using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.MessageExceptions
{
    public class MessageUpdateException : MessageException
    {
        public MessageUpdateException() : base() { }
        public MessageUpdateException(string? message) : base(message) { }
        public MessageUpdateException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
