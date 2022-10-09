namespace AccessToDB.Converters
{
    public class UserConverter
    {
        public static BL.Models.User DBToBL(UserDB user_db)
        {
            return new BL.Models.User(user_db.Item1, user_db.Item2, user_db.Item3, user_db.Item4, (BL.Models.PermissionsEnum) user_db.Item5);
        }

        public static UserDB BLToDB(BL.Models.User user_bl)
        {
            return ValueTuple.Create(user_bl.UserID, user_bl.CardID, user_bl.UserEmail, user_bl.Password, (uint) user_bl.Permissions);
        }

        public static UserDBNoIndex BLToDBNoIndex(BL.Models.User user_bl)
        {
            return ValueTuple.Create(user_bl.CardID, user_bl.UserEmail, user_bl.Password, (uint) user_bl.Permissions);
        }
    }
}
