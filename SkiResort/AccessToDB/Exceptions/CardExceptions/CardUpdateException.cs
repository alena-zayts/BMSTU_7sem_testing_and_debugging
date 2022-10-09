using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.CardExceptions
{
    public class CardUpdateException : CardException
    {
        public uint? cardID;
        public DateTimeOffset? activationTime;
        public string? type;
        public CardUpdateException(uint cardID, DateTimeOffset newActivationTime, string newType)
        {
            this.cardID = cardID;
            this.activationTime = newActivationTime;
            this.type = newType;
        }
    }
}
