using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.CardExceptions
{
    public class CardAddAutoIncrementException : CardException
    {
        public DateTimeOffset? activationTime;
        public string? type;
        public CardAddAutoIncrementException(DateTimeOffset activationTime, string type)
        {
            this.activationTime = activationTime;
            this.type = type;
        }
    }
}
