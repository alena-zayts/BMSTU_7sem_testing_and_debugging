using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.TurnstileExceptions
{
    public class TurnstileDeleteException : TurnstileException
    {
        public TurnstileDeleteException() : base() { }
        public TurnstileDeleteException(string? message) : base(message) { }
        public TurnstileDeleteException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
