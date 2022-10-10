using BL;
using BL.IRepositories;
using AccessToDB.RepositoriesTarantool;
using AccessToDB;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Threading.Tasks;

namespace AccessToDB.Tests.Helpers
{
    public class TarantoolRepositoriesFactory: IRepositoriesFactory
    {
        private TarantoolContext _tarantool_context;

        public TarantoolRepositoriesFactory(TarantoolContext tarantool_context)
        {
            _tarantool_context = tarantool_context;
        }
        public IUsersRepository CreateUsersRepository()
        {
            return new TarantoolUsersRepository(_tarantool_context);
        }
        public ITurnstilesRepository CreateTurnstilesRepository()
        {
            return new TarantoolTurnstilesRepository(_tarantool_context);
        }
        public ISlopesRepository CreateSlopesRepository()
        {
            return new TarantoolSlopesRepository(_tarantool_context);
        }
        public ILiftsRepository CreateLiftsRepository()
        {
            return new TarantoolLiftsRepository(_tarantool_context);
        }
        public ILiftsSlopesRepository CreateLiftsSlopesRepository()
        {
            return new TarantoolLiftsSlopesRepository(_tarantool_context);
        }
    }
}
