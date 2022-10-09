namespace BL.Exceptions.PermissionExceptions
{
    public class PermissionException : Exception
    {
        public uint? UserID { get; }
        public string? FunctionName { get; }

        public PermissionException() : base() { }
        public PermissionException(string? message) : base(message) { }
        public PermissionException(string? message, Exception? innerException) : base(message, innerException) { }

        public PermissionException(string? message, uint? userID, string? functionName) : base(message)
        {
            this.UserID = userID;
            this.FunctionName = functionName;

        }
    }
}
