namespace AccessToDB.Converters
{
    public class TurnstileConverter
    {
        public static BL.Models.Turnstile DBToBL(TurnstileDB db_model)
        {
            return new BL.Models.Turnstile(db_model.Item1, db_model.Item2, db_model.Item3);
        }

        public static TurnstileDB BLToDB(BL.Models.Turnstile bl_model)
        {
            return ValueTuple.Create(bl_model.TurnstileID, bl_model.LiftID, bl_model.IsOpen);
        }

        public static TurnstileDBNoIndex BLToDBNoIndex(BL.Models.Turnstile bl_model)
        {
            return ValueTuple.Create(bl_model.LiftID, bl_model.IsOpen);
        }

    }
}
