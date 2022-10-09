using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.TurnstileExceptions
{
    public class TurnstileUpdateException : TurnstileException
    {
        public TurnstileUpdateException() : base() { }
        public TurnstileUpdateException(string? message) : base(message) { }
        public TurnstileUpdateException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
