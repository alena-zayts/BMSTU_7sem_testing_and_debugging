using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.CardExceptions
{
    public class CardDeleteException : CardException
    {
        public uint? cardID;
        public CardDeleteException(uint cardID)
        {
            this.cardID = cardID;
        }
    }
}
