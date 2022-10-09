using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.CardReadingExceptions
{
    public class CountCardReadingsException : CardReadingException
    {
        public CountCardReadingsException() : base() { }
        public CountCardReadingsException(string? message) : base(message) { }
        public CountCardReadingsException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
