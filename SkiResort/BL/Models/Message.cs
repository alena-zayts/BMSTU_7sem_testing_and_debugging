using BL.Exceptions.MessageExceptions;

namespace BL.Models
{
    public record class Message
    {
        public static uint MessageUniversalID = 0;
        public static uint MessageCheckedByNobody = 0;
        public uint MessageID { get; }
        public uint SenderID { get; }
        public uint CheckedByID { get; }
        public string Text { get; }


        public Message(uint messageID, uint senderID, uint checkedByID, string text)
        {
            if (text.Length == 0)
            {
                throw new MessageCreationException("Text of message cannot be empty");
            }

            this.MessageID = messageID;
            this.SenderID = senderID;
            this.CheckedByID = checkedByID;
            this.Text = text;

        }
    }
}

