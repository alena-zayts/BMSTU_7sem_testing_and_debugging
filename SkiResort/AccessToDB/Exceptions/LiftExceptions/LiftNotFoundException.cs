using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.LiftExceptions
{
    public class LiftNotFoundException : LiftException
    {
        public LiftNotFoundException() : base() { }
        public LiftNotFoundException(string? message) : base(message) { }
        public LiftNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
