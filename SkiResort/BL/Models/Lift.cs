namespace BL.Models
{
    public record class Lift
    {
        public uint LiftID { get; }
        public string LiftName { get; }
        public bool IsOpen { get; }
        public uint SeatsAmount { get; }
        public uint LiftingTime { get;  }
        public uint QueueTime { get;}
        public List<Slope>? ConnectedSlopes { get;}

        public Lift(uint liftID, string liftName, bool isOpen, uint seatsAmount, uint liftingTime, uint queueTime)
        {
            this.LiftID = liftID;
            this.LiftName = liftName;
            this.IsOpen = isOpen;
            this.SeatsAmount = seatsAmount;
            this.LiftingTime = liftingTime;
            this.QueueTime = queueTime;
        }

        public Lift(Lift lift, List<Slope> connectedSlopes)
        {
            this.LiftID = lift.LiftID;
            this.LiftName = lift.LiftName;
            this.IsOpen = lift.IsOpen;
            this.SeatsAmount = lift.SeatsAmount;
            this.LiftingTime = lift.LiftingTime;
            this.QueueTime = lift.QueueTime;
            this.ConnectedSlopes = connectedSlopes;
        }

        public Lift(Lift lift, uint newQueueTime)
        {
            this.LiftID = lift.LiftID;
            this.LiftName = lift.LiftName;
            this.IsOpen = lift.IsOpen;
            this.SeatsAmount = lift.SeatsAmount;
            this.LiftingTime = lift.LiftingTime;
            this.ConnectedSlopes = lift.ConnectedSlopes;

            this.QueueTime = newQueueTime;
        }

        public bool EqualWithoutConnectedSlopes(Lift otherLift)
        {
            return LiftID == otherLift.LiftID &&
                this.LiftName == otherLift.LiftName &&
                this.IsOpen == otherLift.IsOpen &&
                this.SeatsAmount == otherLift.SeatsAmount &&
                this.LiftingTime == otherLift.LiftingTime;
        }
    }
}

