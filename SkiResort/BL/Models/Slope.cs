namespace BL.Models
{
    public record class Slope
    {
        public uint SlopeID { get; }
        public string SlopeName { get;}
        public bool IsOpen { get; }
        public uint DifficultyLevel { get; }
        public List<Lift>? ConnectedLifts { get; }



        public Slope(uint slopeID, string slopeName, bool isOpen, uint difficultyLevel)
        {
            this.SlopeID = slopeID;
            this.SlopeName = slopeName;
            this.IsOpen = isOpen;
            this.DifficultyLevel = difficultyLevel;
        }

        public Slope(Slope slope, List<Lift> connectedLifts)
        {
            this.SlopeID = slope.SlopeID;
            this.SlopeName = slope.SlopeName;
            this.IsOpen = slope.IsOpen;
            this.DifficultyLevel = slope.DifficultyLevel;
            this.ConnectedLifts = connectedLifts;
        }

        public bool EqualWithoutConnectedLifts(Slope otherSlope)
        {
            return this.IsOpen == otherSlope.IsOpen &&
                this.DifficultyLevel == otherSlope.DifficultyLevel &&  
                this.SlopeID == otherSlope.SlopeID &&
                this.SlopeName == otherSlope.SlopeName;

        }
    }
}

