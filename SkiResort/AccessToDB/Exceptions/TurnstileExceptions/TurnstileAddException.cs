using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.TurnstileExceptions
{
    public class TurnstileAddException : TurnstileException
    {
        public TurnstileAddException() : base() { }
        public TurnstileAddException(string? message) : base(message) { }
        public TurnstileAddException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
