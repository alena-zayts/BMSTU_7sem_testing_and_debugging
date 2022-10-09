namespace AccessToDB.Converters
{
    public class LiftConverter
    {
        public static BL.Models.Lift DBToBL(LiftDB db_model)
        {
            return new BL.Models.Lift(db_model.Item1, db_model.Item2, db_model.Item3, db_model.Item4, db_model.Item5, db_model.Item6);
        }

        public static LiftDB BLToDB(BL.Models.Lift bl_model)
        {
            return ValueTuple.Create(bl_model.LiftID, bl_model.LiftName, bl_model.IsOpen, bl_model.SeatsAmount, bl_model.LiftingTime, bl_model.QueueTime);
        }
        public static LiftDBNoIndex BLToDBNoIndex(BL.Models.Lift bl_model)
        {
            return ValueTuple.Create(bl_model.LiftName, bl_model.IsOpen, bl_model.SeatsAmount, bl_model.LiftingTime, bl_model.QueueTime);
        }
    }
}
