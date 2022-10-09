namespace AccessToDB.Converters
{
    public class CardReadingConverter
    {
        public static BL.Models.CardReading DBToBL(CardReadingDB db_model)
        {
            return new BL.Models.CardReading(db_model.Item1, db_model.Item2, db_model.Item3, DateTimeOffset.FromUnixTimeSeconds((long) db_model.Item4));
        }

        public static CardReadingDB BLToDB(BL.Models.CardReading bl_model)
        {
            return ValueTuple.Create(bl_model.RecordID, bl_model.TurnstileID, bl_model.CardID, (uint) bl_model.ReadingTime.ToUnixTimeSeconds());
        }
        public static CardReadingDBNoIndex BLToDBNoIndex(BL.Models.CardReading bl_model)
        {
            return ValueTuple.Create(bl_model.TurnstileID, bl_model.CardID, (uint)bl_model.ReadingTime.ToUnixTimeSeconds());
        }

    }
}
