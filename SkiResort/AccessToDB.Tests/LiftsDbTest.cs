//using System.Threading.Tasks;
//using Xunit;
//using Xunit.Abstractions;

//using ProGaudi.Tarantool.Client;

//using BL.Models;
//using BL.IRepositories;


//using AccessToDB.RepositoriesTarantool;
//using AccessToDB.Exceptions.LiftExceptions;
//using AccessToDB;



//namespace Tests
//{
//    public class LiftsDbTest
//    {
//        TarantoolContext _context;
//        private readonly ITestOutputHelper output;

//        public LiftsDbTest(ITestOutputHelper output)
//        {
//            this.output = output;

//            string connection_string = "ski_admin:Tty454r293300@localhost:3301";
//            _context = new TarantoolContext(connection_string);
//        }

//        [Fact]
//        public async Task Test_Add_GetById_Delete()
//        {
//            ILiftsRepository rep = new TarantoolLiftsRepository(_context);

//            //start testing 
//            Assert.Empty(await rep.GetLiftsAsync());

//            // add correct
//            Lift added_lift = new Lift(1, "A1", true , 10, 100, 1000);
//            await rep.AddLiftAsync(added_lift.LiftID, added_lift.LiftName, added_lift.IsOpen, added_lift.SeatsAmount, added_lift.LiftingTime, added_lift.QueueTime);
//            // add already existing
//            await Assert.ThrowsAsync<LiftAddException>(() => rep.AddLiftAsync(added_lift.LiftID, added_lift.LiftName, added_lift.IsOpen, added_lift.SeatsAmount, added_lift.LiftingTime, added_lift.QueueTime));

//            // get_by_id correct
//            Lift got_lift = await rep.GetLiftByIdAsync(added_lift.LiftID);
//            Assert.Equal(added_lift, got_lift);
//            // get_by_name correct
//            got_lift = await rep.GetLiftByNameAsync(added_lift.LiftName);
//            Assert.Equal(added_lift, got_lift);

//            // delete correct
//            await rep.DeleteLiftByIDAsync(added_lift.LiftID);

//            // get_by_id not existing
//            await Assert.ThrowsAsync<LiftNotFoundException>(() => rep.GetLiftByIdAsync(added_lift.LiftID));
//            // get_by_id incorrect
//            await Assert.ThrowsAsync<LiftNotFoundException>(() => rep.GetLiftByNameAsync(added_lift.LiftName));

//            // delete not existing
//            await Assert.ThrowsAsync<LiftDeleteException>(() => rep.DeleteLiftByIDAsync(added_lift.LiftID));

//            // end tests - empty getlist
//            Assert.Empty(await rep.GetLiftsAsync());
//        }


//        [Fact]
//        public async Task Test_Update_GetList()
//        {

//            ILiftsRepository rep = new TarantoolLiftsRepository(_context);

//            //start testing 
//            Assert.Empty(await rep.GetLiftsAsync());

//            Lift added_lift1 = new Lift(1, "A1", true, 10, 100, 1000);
//            await rep.AddLiftAsync(added_lift1.LiftID, added_lift1.LiftName, added_lift1.IsOpen, added_lift1.SeatsAmount, added_lift1.LiftingTime, added_lift1.QueueTime);

//            Lift added_lift2 = new Lift(2, "B2", false, 20, 200, 2000);
//            await rep.AddLiftAsync(added_lift2.LiftID, added_lift2.LiftName, added_lift2.IsOpen, added_lift2.SeatsAmount, added_lift2.LiftingTime, added_lift2.QueueTime);

//            added_lift2 = new Lift(added_lift2.LiftID, added_lift2.LiftName, added_lift2.IsOpen, 821, added_lift2.LiftingTime, added_lift2.QueueTime);

//            // updates correct
//            await rep.UpdateLiftByIDAsync(added_lift1.LiftID, added_lift1.LiftName, added_lift1.IsOpen, added_lift1.SeatsAmount, added_lift1.LiftingTime);
//            await rep.UpdateLiftByIDAsync(added_lift2.LiftID, added_lift2.LiftName, added_lift2.IsOpen, added_lift2.SeatsAmount, added_lift2.LiftingTime);

//            var list = await rep.GetLiftsAsync();
//            Assert.Equal(2, list.Count);
//            Assert.True(added_lift1.EqualWithoutConnectedSlopes(list[0]));
//            Assert.True(added_lift2.EqualWithoutConnectedSlopes(list[1]));

//            await rep.DeleteLiftByIDAsync(added_lift1.LiftID);
//            await rep.DeleteLiftByIDAsync(added_lift2.LiftID);


//            // updates not existing
//            await Assert.ThrowsAsync<LiftUpdateException>(() => rep.UpdateLiftByIDAsync(added_lift1.LiftID, added_lift1.LiftName, added_lift1.IsOpen, added_lift1.SeatsAmount, added_lift1.LiftingTime));
//            await Assert.ThrowsAsync<LiftUpdateException>(() => rep.UpdateLiftByIDAsync(added_lift2.LiftID, added_lift2.LiftName, added_lift2.IsOpen, added_lift2.SeatsAmount, added_lift2.LiftingTime));


//            // end tests - empty getlist
//            Assert.Empty(await rep.GetLiftsAsync());


//            uint tmpLiftID2 = await rep.AddLiftAutoIncrementAsync(added_lift1.LiftName, added_lift1.IsOpen, added_lift1.SeatsAmount, added_lift1.LiftingTime);
//            Assert.True(1 == tmpLiftID2);
//            uint tmpLiftID3 = await rep.AddLiftAutoIncrementAsync(added_lift2.LiftName, added_lift2.IsOpen, added_lift2.SeatsAmount, added_lift2.LiftingTime);
//            Assert.True(2 == tmpLiftID3);
//            await rep.DeleteLiftByIDAsync(tmpLiftID2);
//            await rep.DeleteLiftByIDAsync(tmpLiftID3);
//            Assert.Empty(await rep.GetLiftsAsync());
//        }
//    }
//}

