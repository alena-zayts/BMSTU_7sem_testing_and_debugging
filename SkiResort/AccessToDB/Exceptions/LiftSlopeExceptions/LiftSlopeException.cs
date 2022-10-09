using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.LiftSlopeExceptions
{
    public class LiftSlopeException : Exception
    {

        public LiftSlopeException() : base() { }
        public LiftSlopeException(string? message) : base(message) { }
        public LiftSlopeException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
