using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.LiftSlopeExceptions
{
    public class LiftSlopeDeleteException : LiftSlopeException
    {
        public LiftSlopeDeleteException() : base() { }
        public LiftSlopeDeleteException(string? message) : base(message) { }
        public LiftSlopeDeleteException(string? message, Exception? innerException) : base(message, innerException) { }


    }
}
