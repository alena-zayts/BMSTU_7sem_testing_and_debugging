using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.LiftSlopeExceptions
{
    public class LiftSlopeAddException : LiftSlopeException
    {
        public LiftSlopeAddException() : base() { }
        public LiftSlopeAddException(string? message) : base(message) { }
        public LiftSlopeAddException(string? message, Exception? innerException) : base(message, innerException) { }


    }
}
