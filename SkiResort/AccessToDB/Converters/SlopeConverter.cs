namespace AccessToDB.Converters
{
    public class SlopeConverter
    {
        public static BL.Models.Slope DBToBL(SlopeDB db_model)
        {
            return new BL.Models.Slope(db_model.Item1, db_model.Item2, db_model.Item3, db_model.Item4);
        }

        public static SlopeDB BLToDB(BL.Models.Slope bl_model)
        {
            return ValueTuple.Create(bl_model.SlopeID, bl_model.SlopeName, bl_model.IsOpen, bl_model.DifficultyLevel);
        }

        public static SlopeDBNoIndex BLToDBNoIndex(BL.Models.Slope bl_model)
        {
            return ValueTuple.Create(bl_model.SlopeName, bl_model.IsOpen, bl_model.DifficultyLevel);
        }
    }
}
