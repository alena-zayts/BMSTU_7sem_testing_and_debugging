using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.TurnstileExceptions
{
    public class TurnstileException : Exception
    {
        public TurnstileException() : base() { }
        public TurnstileException(string? message) : base(message) { }
        public TurnstileException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
