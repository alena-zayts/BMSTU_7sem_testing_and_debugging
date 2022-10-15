using ProGaudi.Tarantool.Client;
using Microsoft.Extensions.Configuration;


namespace AccessToDB
{
    public class TarantoolContext
    {
        public IBox box;
        public ISpace liftsSpace;
        public IIndex liftsIndexPrimary;
        public IIndex liftsIndexName;

        public ISpace slopesSpace;
        public IIndex slopesIndexPrimary;
        public IIndex slopesIndexName;

        public ISpace liftsSlopesSpace;
        public IIndex liftsSlopesIndexPrimary;
        public IIndex liftsSlopesIndexLiftID;
        public IIndex liftsSlopesIndexSlopeID;

        public ISpace turnstilesSpace;
        public IIndex turnstilesIndexPrimary;
        public IIndex turnstilesIndexLiftID;

        public ISpace usersSpace;
        public IIndex users_indexPrimary;
        public IIndex users_index_email;


        public TarantoolContext(string connection_string) => (
            box,
            liftsSpace, liftsIndexPrimary, liftsIndexName,
            slopesSpace, slopesIndexPrimary, slopesIndexName,
            liftsSlopesSpace, liftsSlopesIndexPrimary, liftsSlopesIndexLiftID, liftsSlopesIndexSlopeID,
            turnstilesSpace, turnstilesIndexPrimary, turnstilesIndexLiftID,
            usersSpace, users_indexPrimary, users_index_email
            ) = Initialize(connection_string).GetAwaiter().GetResult();


        private static async Task<(
        IBox,
        ISpace, IIndex, IIndex,
        ISpace, IIndex, IIndex,
        ISpace, IIndex, IIndex, IIndex,
        ISpace, IIndex, IIndex,
        ISpace, IIndex, IIndex)> 
            Initialize(string connection_string)
        {
            Box box = null;
            while (box == null)
            {
                try
                {
                    box = await Box.Connect(connection_string);
                }
                catch
                {

                }
            }


            var schema = box.GetSchema();

            var liftsSpace = await schema.GetSpace("lifts");
            var liftsIndexPrimary = await liftsSpace.GetIndex("primary");
            var liftsIndexName = await liftsSpace.GetIndex("index_name");

            var slopesSpace = await schema.GetSpace("slopes");
            var slopesIndexPrimary = await slopesSpace.GetIndex("primary");
            var slopesIndexName = await slopesSpace.GetIndex("index_name");

            var liftsSlopesSpace = await schema.GetSpace("lifts_slopes");
            var liftsSlopesIndexPrimary = await liftsSlopesSpace.GetIndex("primary");
            var liftsSlopesIndexLiftID = await liftsSlopesSpace.GetIndex("index_lift_id");
            var liftsSlopesIndexSlopeID = await liftsSlopesSpace.GetIndex("index_slope_id");


            var turnstilesSpace = await schema.GetSpace("turnstiles");
            var turnstilesIndexPrimary = await turnstilesSpace.GetIndex("primary");
            var turnstilesIndexLiftID = await turnstilesSpace.GetIndex("index_lift_id");


            var usersSpace = await schema.GetSpace("users");
            var users_indexPrimary = await usersSpace.GetIndex("primary");
            var users_index_email = await usersSpace.GetIndex("index_email");


            return (
                box,
                liftsSpace, liftsIndexPrimary, liftsIndexName,
                slopesSpace, slopesIndexPrimary, slopesIndexName,
                liftsSlopesSpace, liftsSlopesIndexPrimary, liftsSlopesIndexLiftID, liftsSlopesIndexSlopeID,
                turnstilesSpace, turnstilesIndexPrimary, turnstilesIndexLiftID,
                usersSpace, users_indexPrimary, users_index_email
            );
        }

        public void Dispose()
        {
            box.Dispose();
            //System.Threading.Thread.Sleep(1000);
        }
    }
}

