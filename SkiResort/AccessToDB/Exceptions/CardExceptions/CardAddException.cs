using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.CardExceptions
{
    public class CardAddException : CardException
    {
        public uint? cardID;
        public DateTimeOffset? activationTime;
        public string? type;
        public CardAddException(uint cardID, DateTimeOffset activationTime, string type)
        {
            this.cardID = cardID;
            this.activationTime = activationTime;
            this.type = type;
        }
    }
}
