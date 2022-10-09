using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.SlopeExceptions
{
    public class SlopeException : Exception
    {
        public SlopeException() : base() { }
        public SlopeException(string? message) : base(message) { }
        public SlopeException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
