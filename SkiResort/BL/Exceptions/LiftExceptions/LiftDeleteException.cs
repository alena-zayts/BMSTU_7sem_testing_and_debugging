using BL.Models;

namespace BL.Exceptions.LiftExceptions
{
    public class LiftDeleteException : Exception
    {
        public Lift? Lift { get; }
        public LiftDeleteException() : base() { }
        public LiftDeleteException(string? message) : base(message) { }
        public LiftDeleteException(string? message, Exception? innerException) : base(message, innerException) { }

        public LiftDeleteException(string? message, Lift? lift) : base(message)
        {
            this.Lift = lift;
        }

    }
}

