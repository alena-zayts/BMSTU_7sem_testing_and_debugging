using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.MessageExceptions
{

    public class MessageAddException : MessageException
    {
        public MessageAddException() : base() { }
        public MessageAddException(string? message) : base(message) { }
        public MessageAddException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
