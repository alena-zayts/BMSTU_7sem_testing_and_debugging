using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.CardReadingExceptions
{
    public class CardReadingAddException : CardReadingException
    {
        public CardReadingAddException() : base() { }
        public CardReadingAddException(string? message) : base(message) { }
        public CardReadingAddException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
