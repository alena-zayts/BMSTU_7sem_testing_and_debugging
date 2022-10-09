//using System.Threading.Tasks;
//using Xunit;
//using Xunit.Abstractions;

//using ProGaudi.Tarantool.Client;

//using BL.Models;
//using BL.IRepositories;


//using AccessToDB.RepositoriesTarantool;
//using AccessToDB.Exceptions.SlopeExceptions;
//using AccessToDB;



//namespace Tests
//{
//    public class TestSlopesRepository
//    {
//        TarantoolContext _context;
//        private readonly ITestOutputHelper output;

//        public TestSlopesRepository(ITestOutputHelper output)
//        {
//            this.output = output;

//            string connection_string = "ski_admin:Tty454r293300@localhost:3301";
//            _context = new TarantoolContext(connection_string);
//        }

//        [Fact]
//        public async Task Test_Add_GetById_Delete()
//        {
//            ISlopesRepository rep = new TarantoolSlopesRepository(_context);

//            //start testing 
//            Assert.Empty(await rep.GetSlopesAsync());

//            // add correct
//            Slope added_slope = new Slope(1, "A1", true, 10);
//            await rep.AddSlopeAsync(added_slope.SlopeID, added_slope.SlopeName, added_slope.IsOpen, added_slope.DifficultyLevel);
//            // add already existing
//            await Assert.ThrowsAsync<SlopeAddException>(() => rep.AddSlopeAsync(added_slope.SlopeID, added_slope.SlopeName, added_slope.IsOpen, added_slope.DifficultyLevel));

//            // get_by_id correct
//            Slope got_slope = await rep.GetSlopeByIdAsync(added_slope.SlopeID);
//            Assert.Equal(added_slope, got_slope);
//            // get_by_name correct
//            got_slope = await rep.GetSlopeByNameAsync(added_slope.SlopeName);
//            Assert.Equal(added_slope, got_slope);

//            // delete correct
//            await rep.DeleteSlopeByIDAsync(added_slope.SlopeID);

//            // get_by_id not existing
//            await Assert.ThrowsAsync<SlopeNotFoundException>(() => rep.GetSlopeByIdAsync(added_slope.SlopeID));
//            // get_by_id incorrect
//            await Assert.ThrowsAsync<SlopeNotFoundException>(() => rep.GetSlopeByNameAsync(added_slope.SlopeName));

//            // delete not existing
//            await Assert.ThrowsAsync<SlopeDeleteException>(() => rep.DeleteSlopeByIDAsync(added_slope.SlopeID));

//            // end tests - empty getlist
//            Assert.Empty(await rep.GetSlopesAsync());
//        }


//        [Fact]
//        public async Task Test_Update_GetList()
//        {

//            ISlopesRepository rep = new TarantoolSlopesRepository(_context);

//            //start testing 
//            Assert.Empty(await rep.GetSlopesAsync());

//            Slope added_slope1 = new Slope(1, "A1", true, 10);
//            await rep.AddSlopeAsync(added_slope1.SlopeID, added_slope1.SlopeName, added_slope1.IsOpen, added_slope1.DifficultyLevel);

//            Slope added_slope2 = new Slope(2, "B2", false, 20);
//            await rep.AddSlopeAsync(added_slope2.SlopeID, added_slope2.SlopeName, added_slope2.IsOpen, added_slope2.DifficultyLevel);

//            added_slope2 = new Slope(added_slope2.SlopeID, "dfd", added_slope2.IsOpen, added_slope2.DifficultyLevel);

//            // updates correct
//            await rep.UpdateSlopeByIDAsync(added_slope1.SlopeID, added_slope1.SlopeName, added_slope1.IsOpen, added_slope1.DifficultyLevel);
//            await rep.UpdateSlopeByIDAsync(added_slope2.SlopeID, added_slope2.SlopeName, added_slope2.IsOpen, added_slope2.DifficultyLevel);

//            var list = await rep.GetSlopesAsync();
//            Assert.Equal(2, list.Count);
//            Assert.Equal(added_slope1, list[0]);
//            Assert.Equal(added_slope2, list[1]);

//            await rep.DeleteSlopeByIDAsync(added_slope1.SlopeID);
//            await rep.DeleteSlopeByIDAsync(added_slope2.SlopeID);


//            // updates not existing
//            await Assert.ThrowsAsync<SlopeUpdateException>(() => rep.UpdateSlopeByIDAsync(added_slope1.SlopeID, added_slope1.SlopeName, added_slope1.IsOpen, added_slope1.DifficultyLevel));
//            await Assert.ThrowsAsync<SlopeUpdateException>(() => rep.UpdateSlopeByIDAsync(added_slope2.SlopeID, added_slope2.SlopeName, added_slope2.IsOpen, added_slope2.DifficultyLevel));


//            // end tests - empty getlist
//            Assert.Empty(await rep.GetSlopesAsync());


//            uint tmpSlopeID2 = await rep.AddSlopeAutoIncrementAsync(added_slope1.SlopeName, added_slope1.IsOpen, added_slope1.DifficultyLevel);
//            Assert.True(1 == tmpSlopeID2);
//            uint tmptmpSlopeID3 = await rep.AddSlopeAutoIncrementAsync(added_slope2.SlopeName, added_slope2.IsOpen, added_slope2.DifficultyLevel);
//            Assert.True(2 == tmptmpSlopeID3);
//            await rep.DeleteSlopeByIDAsync(tmpSlopeID2);
//            await rep.DeleteSlopeByIDAsync(tmptmpSlopeID3);
//            Assert.Empty(await rep.GetSlopesAsync());
//        }
//    }
//}


