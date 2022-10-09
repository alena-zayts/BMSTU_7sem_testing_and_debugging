using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.LiftExceptions
{
    public class LiftAddException : LiftException
    {
        public LiftAddException() : base() { }
        public LiftAddException(string? message) : base(message) { }
        public LiftAddException(string? message, Exception? innerException) : base(message, innerException) { }


    }
}
