using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.LiftExceptions
{
    public class LiftAddAutoIncrementException : LiftException
    {
        public LiftAddAutoIncrementException() : base() { }
        public LiftAddAutoIncrementException(string? message) : base(message) { }
        public LiftAddAutoIncrementException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
