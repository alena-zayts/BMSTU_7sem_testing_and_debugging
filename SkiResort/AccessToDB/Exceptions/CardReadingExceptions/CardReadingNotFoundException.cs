using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.CardReadingExceptions
{
    public class CardReadingNotFoundException : CardReadingException
    {
        public uint? recordID;
        public CardReadingNotFoundException(uint recordID)
        {
            this.recordID = recordID;
        }
    }
}
