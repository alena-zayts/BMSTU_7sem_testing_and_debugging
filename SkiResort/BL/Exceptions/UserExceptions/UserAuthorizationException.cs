using BL.Models;

namespace BL.Exceptions.UserExceptions
{
    public class UserAuthorizationException : Exception
    {
        public User? User { get; }
        public UserAuthorizationException() : base() { }
        public UserAuthorizationException(string? message) : base(message) { }
        public UserAuthorizationException(string? message, Exception? innerException) : base(message, innerException) { }

        public UserAuthorizationException(string? message, User? user) : base(message)
        {
            this.User = user;
        }
    }
}
