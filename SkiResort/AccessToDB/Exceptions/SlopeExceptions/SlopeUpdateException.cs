using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.SlopeExceptions
{
    public class SlopeUpdateException : SlopeException
    {
        public SlopeUpdateException() : base() { }
        public SlopeUpdateException(string? message) : base(message) { }
        public SlopeUpdateException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
