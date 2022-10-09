namespace BL.Models
{
    public enum PermissionsEnum: uint
    {
        UNAUTHORIZED = 0u,
        AUTHORIZED = 1u,
        SKI_PATROL = 2u,
        ADMIN = 3u
    }

    public record class User
    {
        public const uint UniversalCardID = 0;

        public const uint UniversalUserID = 0;

        public const uint UnauthorizedUserID = 9999;
        public uint UserID { get; }
        public uint CardID { get; }
        public string UserEmail { get; }
        public string Password { get; }
        public PermissionsEnum Permissions { get; }

        public User(uint userID, uint cardID, string UserEmail, string password, PermissionsEnum permissions)
        {
            this.UserID = userID;
            this.CardID = cardID;
            this.UserEmail = UserEmail;
            this.Password = password;
            this.Permissions = permissions;
        }
    }
}

