
namespace BL.Models
{
    public record class Card
    {
        public uint CardID { get; }
        public DateTimeOffset ActivationTime { get; }
        public string Type { get; }

        public Card(uint cardID, DateTimeOffset activationTime, string type)
        {
            this.CardID = cardID;
            this.ActivationTime = activationTime;
            this.Type = type;

        }
    }
}

