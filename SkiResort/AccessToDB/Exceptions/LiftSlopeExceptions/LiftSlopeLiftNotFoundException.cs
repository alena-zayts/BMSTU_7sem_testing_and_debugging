using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.LiftSlopeExceptions
{
    public class LiftSlopeLiftNotFoundException : LiftSlopeException
    {

        public LiftSlopeLiftNotFoundException() : base() { }
        public LiftSlopeLiftNotFoundException(string? message) : base(message) { }
        public LiftSlopeLiftNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
