using BL.IRepositories;

namespace AccessToDB.Tests.Helpers
{
    public interface IRepositoriesFactory
    {
        public IUsersRepository CreateUsersRepository();
        public ITurnstilesRepository CreateTurnstilesRepository();
        public ISlopesRepository CreateSlopesRepository();
        public ILiftsRepository CreateLiftsRepository();
        public ILiftsSlopesRepository CreateLiftsSlopesRepository();
    }
}