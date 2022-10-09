using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.SlopeExceptions
{
    public class SlopeDeleteException : SlopeException
    {
        public SlopeDeleteException() : base() { }
        public SlopeDeleteException(string? message) : base(message) { }
        public SlopeDeleteException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
