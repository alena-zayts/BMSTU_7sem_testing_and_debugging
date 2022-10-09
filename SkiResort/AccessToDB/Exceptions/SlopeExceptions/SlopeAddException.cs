using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.SlopeExceptions
{

    public class SlopeAddException : SlopeException
    {
        public SlopeAddException() : base() { }
        public SlopeAddException(string? message) : base(message) { }
        public SlopeAddException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
