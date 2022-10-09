using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.MessageExceptions
{
    public class MessageException : Exception
    {

        public MessageException() : base() { }
        public MessageException(string? message) : base(message) { }
        public MessageException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
