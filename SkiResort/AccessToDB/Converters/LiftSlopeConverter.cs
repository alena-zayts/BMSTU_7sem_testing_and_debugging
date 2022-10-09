namespace AccessToDB.Converters
{
    public class LiftSlopeConverter
    {
        public static BL.Models.LiftSlope DBToBL(LiftSlopeDB db_model)
        {
            return new BL.Models.LiftSlope(db_model.Item1, db_model.Item2, db_model.Item3);
        }

        public static LiftSlopeDB BLToDB(BL.Models.LiftSlope bl_model)
        {
            return ValueTuple.Create(bl_model.RecordID, bl_model.LiftID, bl_model.SlopeID);
        }

        public static LiftSlopeDBNoIndex BLToDBNoIndex(BL.Models.LiftSlope bl_model)
        {
            return ValueTuple.Create(bl_model.LiftID, bl_model.SlopeID);
        }
    }
}
