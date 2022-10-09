using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.CardExceptions
{
    public class CardNotFoundException : CardException
    {
        public uint? cardID;
        public CardNotFoundException(uint cardID)
        {
            this.cardID = cardID;
        }
    }
}
