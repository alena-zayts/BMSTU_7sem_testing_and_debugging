using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.UserExceptions
{
    public class UserDeleteException : UserException
    {
        public UserDeleteException() : base() { }
        public UserDeleteException(string? message) : base(message) { }
        public UserDeleteException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
