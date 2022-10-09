using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.MessageExceptions
{
    public class MessageAddAutoIncrementException : MessageException
    {
        public MessageAddAutoIncrementException() : base() { }
        public MessageAddAutoIncrementException(string? message) : base(message) { }
        public MessageAddAutoIncrementException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
