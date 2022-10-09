using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.CardReadingExceptions
{
    public class CardReadingException : Exception
    {

        public CardReadingException() : base() { }
        public CardReadingException(string? message) : base(message) { }
        public CardReadingException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
