using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.LiftExceptions
{
    public class LiftUpdateException : LiftException
    {
        public LiftUpdateException() : base() { }
        public LiftUpdateException(string? message) : base(message) { }
        public LiftUpdateException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
