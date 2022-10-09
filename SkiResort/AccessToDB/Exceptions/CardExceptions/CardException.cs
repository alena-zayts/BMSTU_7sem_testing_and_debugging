using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.CardExceptions
{
    public class CardException : Exception
    {
        public CardException() : base() { }
        public CardException(string? message) : base(message) { }
        public CardException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
