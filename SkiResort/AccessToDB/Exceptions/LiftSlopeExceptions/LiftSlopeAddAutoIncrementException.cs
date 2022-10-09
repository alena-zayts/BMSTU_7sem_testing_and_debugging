using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.LiftSlopeExceptions
{
    public class LiftSlopeAddAutoIncrementException : LiftSlopeException
    {
        public LiftSlopeAddAutoIncrementException() : base() { }
        public LiftSlopeAddAutoIncrementException(string? message) : base(message) { }
        public LiftSlopeAddAutoIncrementException(string? message, Exception? innerException) : base(message, innerException) { }


    }
}
