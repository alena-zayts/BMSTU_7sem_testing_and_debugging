using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToDB.Exceptions.UserExceptions
{

    public class UserAddAutoIncrementException : UserException
    {
        public UserAddAutoIncrementException() : base() { }
        public UserAddAutoIncrementException(string? message) : base(message) { }
        public UserAddAutoIncrementException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
