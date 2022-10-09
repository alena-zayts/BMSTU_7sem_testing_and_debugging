using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.UserExceptions
{
    public class UserUpdateException : UserException
    {
        public UserUpdateException() : base() { }
        public UserUpdateException(string? message) : base(message) { }
        public UserUpdateException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
