using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.LiftExceptions
{
    public class LiftDeleteException : LiftException
    {
        public LiftDeleteException() : base() { }
        public LiftDeleteException(string? message) : base(message) { }
        public LiftDeleteException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
