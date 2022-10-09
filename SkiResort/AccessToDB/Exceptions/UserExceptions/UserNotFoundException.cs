using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.UserExceptions
{
    public class UserNotFoundException : UserException
    {
        public UserNotFoundException() : base() { }
        public UserNotFoundException(string? message) : base(message) { }
        public UserNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
