using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.LiftSlopeExceptions
{
    public class LiftSlopeUpdateException : LiftSlopeException
    {
        public LiftSlopeUpdateException() : base() { }
        public LiftSlopeUpdateException(string? message) : base(message) { }
        public LiftSlopeUpdateException(string? message, Exception? innerException) : base(message, innerException) { }


    }
}
