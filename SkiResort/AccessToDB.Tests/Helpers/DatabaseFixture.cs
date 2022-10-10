//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using AccessToDB;
//using Xunit;

//namespace AccessToDB.Tests
//{
//    public class DatabaseFixture : IDisposable
//    {
//        public DatabaseFixture()
//        {
//            Db = new TarantoolContext("ski_admin:Tty454r293300@localhost:3301");

//            // ... initialize data in the test database ...
//        }

//        public void Dispose()
//        {
//            // ... clean up test data from the database ...
//        }

//        public TarantoolContext Db { get; private set; }
//    }

//    [CollectionDefinition("Tarantool Database Collection")]
//    public class TarantoolDatabaseCollection : ICollectionFixture<DatabaseFixture>
//    {
//        // This class has no code, and is never created. Its purpose is simply
//        // to be the place to apply [CollectionDefinition] and all the
//        // ICollectionFixture<> interfaces.
//    }
//    public class StackTests
//    {
//        public class EmptyStack
//        {
//            Stack<int> stack;

//            public EmptyStack()
//            {
//                stack = new Stack<int>();
//            }

//            // ... tests for an empty stack ...
//        }

//        public class SingleItemStack
//        {
//            Stack<int> stack;

//            public SingleItemStack()
//            {
//                stack = new Stack<int>();
//                stack.Push(42);
//            }

//            // ... tests for a single-item stack ...
//        }
//    }
//}
