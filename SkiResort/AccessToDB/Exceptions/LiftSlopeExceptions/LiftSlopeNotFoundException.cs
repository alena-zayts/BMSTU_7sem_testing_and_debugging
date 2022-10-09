using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.LiftSlopeExceptions
{
    public class LiftSlopeNotFoundException : LiftSlopeException
    {
        public LiftSlopeNotFoundException() : base() { }
        public LiftSlopeNotFoundException(string? message) : base(message) { }
        public LiftSlopeNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }


    }
}
