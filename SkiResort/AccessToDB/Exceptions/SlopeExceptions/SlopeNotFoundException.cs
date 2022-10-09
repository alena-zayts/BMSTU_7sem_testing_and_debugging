using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.SlopeExceptions
{

    public class SlopeNotFoundException : SlopeException
    {
        public SlopeNotFoundException() : base() { }
        public SlopeNotFoundException(string? message) : base(message) { }
        public SlopeNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
