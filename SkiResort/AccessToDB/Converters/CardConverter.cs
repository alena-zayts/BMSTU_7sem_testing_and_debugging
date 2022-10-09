
namespace AccessToDB.Converters
{
    public class CardConverter
    {
        public static BL.Models.Card DBToBL(CardDB user_db)
        {
            //return new BL.Models.Card(user_db.Item1, new DateTimeOffset((int) user_db.Item2, TimeSpan.Zero), user_db.Item3);
            return new BL.Models.Card(user_db.Item1, DateTimeOffset.FromUnixTimeSeconds(user_db.Item2), user_db.Item3);
        }

        public static CardDB BLToDB(BL.Models.Card card_bl)
        {
            return ValueTuple.Create(card_bl.CardID, (uint) card_bl.ActivationTime.ToUnixTimeSeconds(), card_bl.Type);
        }
        public static CardDBNoIndex BLToDBNoIndex(BL.Models.Card card_bl)
        {
            return ValueTuple.Create((uint)card_bl.ActivationTime.ToUnixTimeSeconds(), card_bl.Type);
        }
    }
}
