using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.CardReadingExceptions
{
    public class CardReadingUpdateException : CardReadingException
    {
        public uint? recordID;
        public uint? turnstileID;
        public uint? cardID;
        public DateTimeOffset? readingTime;
        public CardReadingUpdateException(uint recordID, uint turnstileID, uint cardID, DateTimeOffset readingTime)
        {
            this.recordID = recordID;
            this.cardID = cardID;
            this.turnstileID = turnstileID;
            this.readingTime = readingTime;
        }
    }
}
