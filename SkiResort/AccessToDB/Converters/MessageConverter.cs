namespace AccessToDB.Converters
{
    public class MessageConverter
    {
        public static BL.Models.Message DBToBL(MessageDB db_model)
        {
            return new BL.Models.Message(db_model.Item1, db_model.Item2, db_model.Item3, db_model.Item4);
        }

        public static MessageDB BLToDB(BL.Models.Message bl_model)
        {
            return ValueTuple.Create(bl_model.MessageID, bl_model.SenderID, bl_model.CheckedByID, bl_model.Text);
        }
        public static MessageDBNoIndex BLToDBNoIndex(BL.Models.Message bl_model)
        {
            return ValueTuple.Create(bl_model.SenderID, bl_model.CheckedByID, bl_model.Text);
        }
    }
}
