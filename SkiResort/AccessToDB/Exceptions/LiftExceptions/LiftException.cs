using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.LiftExceptions
{
    public class LiftException : Exception
    {

        public LiftException() : base() { }
        public LiftException(string? message) : base(message) { }
        public LiftException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
