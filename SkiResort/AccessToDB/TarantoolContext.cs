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

        public ISpace cardReadingsSpace;
        public IIndex cardReadingsIndexPrimary;
        public IIndex cardReadingsIndexTurnstile;

        public ISpace cardsSpace;
        public IIndex cardsIndexPrimary;

        public ISpace usersSpace;
        public IIndex users_indexPrimary;
        public IIndex users_index_email;

        public ISpace messagesSpace;
        public IIndex messagesIndexPrimary;
        public IIndex messagesIndexSenderID;
        public IIndex messagesIndexCheckedByID;


        public TarantoolContext(string? connection_string=null) => (
            box,
            liftsSpace, liftsIndexPrimary, liftsIndexName,
            slopesSpace, slopesIndexPrimary, slopesIndexName,
            liftsSlopesSpace, liftsSlopesIndexPrimary, liftsSlopesIndexLiftID, liftsSlopesIndexSlopeID,
            turnstilesSpace, turnstilesIndexPrimary, turnstilesIndexLiftID,
            cardReadingsSpace, cardReadingsIndexPrimary, cardReadingsIndexTurnstile,
            cardsSpace, cardsIndexPrimary,
            usersSpace, users_indexPrimary, users_index_email,
            messagesSpace, messagesIndexPrimary, messagesIndexSenderID, messagesIndexCheckedByID
            ) = Initialize(connection_string).GetAwaiter().GetResult();


        private static async Task<(
        IBox,
        ISpace, IIndex, IIndex,
        ISpace, IIndex, IIndex,
        ISpace, IIndex, IIndex, IIndex,
        ISpace, IIndex, IIndex,
        ISpace, IIndex, IIndex,
        ISpace, IIndex,
        ISpace, IIndex, IIndex,
        ISpace, IIndex, IIndex, IIndex)> 
            Initialize(string? connection_string)
        {
            if (connection_string == null)
            {
                var configurationBuilder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables();
                var config = configurationBuilder.Build();
                connection_string = config["Connections:ConnectAsAdmin"];
            }

            var box = await Box.Connect(connection_string);
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


            var cardReadingsSpace = await schema.GetSpace("card_readings");
            var cardReadingsIndexPrimary = await cardReadingsSpace.GetIndex("primary");
            var cardReadingsIndexTurnstile = await cardReadingsSpace.GetIndex("index_turnstile");


            var cardsSpace = await schema.GetSpace("cards");
            var cardsIndexPrimary = await cardsSpace.GetIndex("primary");


            var usersSpace = await schema.GetSpace("users");
            var users_indexPrimary = await usersSpace.GetIndex("primary");
            var users_index_email = await usersSpace.GetIndex("index_email");


            var messagesSpace = await schema.GetSpace("messages");
            var messagesIndexPrimary = await messagesSpace.GetIndex("primary");
            var messagesIndexSenderID = await messagesSpace.GetIndex("index_sender_id");
            var messagesIndexCheckedByID = await messagesSpace.GetIndex("index_checked_by_id");




            return (
                box,
                liftsSpace, liftsIndexPrimary, liftsIndexName,
                slopesSpace, slopesIndexPrimary, slopesIndexName,
                liftsSlopesSpace, liftsSlopesIndexPrimary, liftsSlopesIndexLiftID, liftsSlopesIndexSlopeID,
                turnstilesSpace, turnstilesIndexPrimary, turnstilesIndexLiftID,
                cardReadingsSpace, cardReadingsIndexPrimary, cardReadingsIndexTurnstile,
                cardsSpace, cardsIndexPrimary,
                usersSpace, users_indexPrimary, users_index_email,
                messagesSpace, messagesIndexPrimary, messagesIndexSenderID, messagesIndexCheckedByID
            );
        }
    }
}

