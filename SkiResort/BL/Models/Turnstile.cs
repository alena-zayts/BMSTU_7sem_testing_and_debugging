namespace BL.Models
{
    public record class Turnstile
    {
        public uint TurnstileID { get; }
        public uint LiftID { get; }
        public bool IsOpen { get; }

        public Turnstile(uint turnstileID, uint liftID, bool isOpen)
        {
            this.TurnstileID = turnstileID;
            this.LiftID = liftID;
            this.IsOpen = isOpen;

        }
    }
}

