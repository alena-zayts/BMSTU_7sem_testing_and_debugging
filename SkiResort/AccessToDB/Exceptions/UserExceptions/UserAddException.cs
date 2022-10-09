using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.UserExceptions
{
    public class UserAddException : UserException
    {
        public UserAddException() : base() { }
        public UserAddException(string? message) : base(message) { }
        public UserAddException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
