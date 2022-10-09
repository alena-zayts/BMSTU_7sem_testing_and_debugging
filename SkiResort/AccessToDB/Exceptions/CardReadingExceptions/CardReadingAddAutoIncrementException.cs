using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.CardReadingExceptions
{
    public class CardReadingAddAutoIncrementException : CardReadingException
    {
        public CardReadingAddAutoIncrementException() : base() { }
        public CardReadingAddAutoIncrementException(string? message) : base(message) { }
        public CardReadingAddAutoIncrementException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
