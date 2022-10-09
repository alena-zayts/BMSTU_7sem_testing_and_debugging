using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.TurnstileExceptions
{
    public class TurnstileAddAutoIncrementException : TurnstileException
    {
        public TurnstileAddAutoIncrementException() : base() { }
        public TurnstileAddAutoIncrementException(string? message) : base(message) { }
        public TurnstileAddAutoIncrementException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
