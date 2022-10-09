using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.LiftSlopeExceptions
{
    public class LiftSlopeSlopeNotFoundException : LiftSlopeException
    {

        public LiftSlopeSlopeNotFoundException() : base() { }
        public LiftSlopeSlopeNotFoundException(string? message) : base(message) { }
        public LiftSlopeSlopeNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }


    }
}
