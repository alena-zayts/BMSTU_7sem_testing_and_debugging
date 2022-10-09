using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.CardReadingExceptions
{
    public class CardReadingDeleteException : CardReadingException
    {
        public CardReadingDeleteException() : base() { }
        public CardReadingDeleteException(string? message) : base(message) { }
        public CardReadingDeleteException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
