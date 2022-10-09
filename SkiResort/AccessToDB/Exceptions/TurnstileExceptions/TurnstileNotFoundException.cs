using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.TurnstileExceptions
{
    public class TurnstileNotFoundException : TurnstileException
    {
        public TurnstileNotFoundException() : base() { }
        public TurnstileNotFoundException(string? message) : base(message) { }
        public TurnstileNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
