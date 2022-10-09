using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.SlopeExceptions
{
    public class SlopeAddAutoIncrementException : SlopeException
    {
        public SlopeAddAutoIncrementException() : base() { }
        public SlopeAddAutoIncrementException(string? message) : base(message) { }
        public SlopeAddAutoIncrementException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
