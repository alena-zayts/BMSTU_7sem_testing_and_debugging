using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.MessageExceptions
{
    public class MessageNotFoundException : MessageException
    {
        public MessageNotFoundException() : base() { }
        public MessageNotFoundException(string? message) : base(message) { }
        public MessageNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
