namespace BL.Models
{
    public record class CardReading
    {
        public uint RecordID { get; }
        public uint TurnstileID { get; }
        public uint CardID { get; }
        public DateTimeOffset ReadingTime { get; }

        public CardReading(uint recordID, uint turnstileID, uint cardID, DateTimeOffset readingTime)
        {
            this.RecordID = recordID;
            this.TurnstileID = turnstileID;
            this.CardID = cardID;
            this.ReadingTime = readingTime;

        }
    }
}

