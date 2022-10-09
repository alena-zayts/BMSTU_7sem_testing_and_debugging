using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;
using ProGaudi.Tarantool.Client.Model.Enums;
using ProGaudi.Tarantool.Client.Model.UpdateOperations;

using BL;
using BL.Models;
using BL.IRepositories;
using AccessToDB.Converters;
using AccessToDB.Exceptions.UserExceptions;

namespace AccessToDB.RepositoriesTarantool
{
    public class TarantoolUsersRepository : IUsersRepository
    {
        private ISpace _space;
        private IIndex _indexPrimary;
        private IIndex _indexEmail;
        private IBox _box;

        public TarantoolUsersRepository(TarantoolContext context)
        {
            _space = context.usersSpace;
            _indexPrimary = context.users_indexPrimary;
            _indexEmail = context.users_index_email;
            _box = context.box;
        }

        public async Task<List<User>> GetUsersAsync(uint offset = 0u, uint limit = 0)
        {
            var data = await _indexPrimary.Select<ValueTuple<uint>, UserDB>
                (ValueTuple.Create(0u), new SelectOptions { Iterator = Iterator.Ge });

            List<User> result = new();

            for (uint i = offset; i < (uint)data.Data.Length && (i < limit || limit == 0); i++)
            {
                result.Add(UserConverter.DBToBL(data.Data[i]));
            }

            return result;
        }

        public async Task<User> GetUserByIdAsync(uint UserID)
        {
            if (UserID == BL.Models.User.UnauthorizedUserID)
            {
                return new User(UserID, 0, $"{UserID}", $"{UserID}", PermissionsEnum.UNAUTHORIZED);
            }

            var data = await _indexPrimary.Select<ValueTuple<uint>,UserDB>
                (ValueTuple.Create(UserID));

            if (data.Data.Length != 1)
            {
                throw new UserNotFoundException();
            }

            return UserConverter.DBToBL(data.Data[0]);
        }

        public async Task AddUserAsync(uint userID, uint cardID, string UserEmail, string password, PermissionsEnum permissions)
        {
            try
            {
                await _space.Insert(new UserDB(userID, cardID, UserEmail, password, (uint) permissions));
            }
            catch (Exception ex)
            {
                throw new UserAddException();
            }
        }

        public async Task<uint> AddUserAutoIncrementAsync(uint cardID, string UserEmail, string password, PermissionsEnum permissions)
        {
            var tmp = await CheckUserEmailExistsAsync(UserEmail);
            try
            {
                var result = await _box.Call_1_6<UserDBNoIndex, UserDB>("auto_increment_users", (new UserDBNoIndex(cardID, UserEmail, password, (uint)permissions)));
                return UserConverter.DBToBL(result.Data[0]).UserID;
            }
            catch (Exception ex)
            {
                throw new UserAddAutoIncrementException();
            }
        }

        public async Task UpdateUserByIDAsync(uint userID, uint newCardID, string newUserEmail, string newPassword, PermissionsEnum newPermissions)
        {
            var response = await _space.Update<ValueTuple<uint>, UserDB>(
                ValueTuple.Create(userID), new UpdateOperation[] {
                    UpdateOperation.CreateAssign<uint>(1, newCardID),
                    UpdateOperation.CreateAssign<string>(2, newUserEmail),
                    UpdateOperation.CreateAssign<string>(3, newPassword),
                    UpdateOperation.CreateAssign<uint>(4, (uint)newPermissions),
                });

            if (response.Data.Length != 1)
            {
                throw new UserUpdateException();
            }
        }

        public async Task DeleteUserByIDAsync(uint userID)
        {
            var response = await _indexPrimary.Delete<ValueTuple<uint>,UserDB>
                (ValueTuple.Create(userID));

            if (response.Data.Length != 1)
            {
                throw new UserDeleteException();
            }

        }
        public async Task<bool> CheckUserIdExistsAsync(uint UserID)
        {
            try
            {
                User user_tmp = await GetUserByIdAsync(UserID);
                return true;
            }
            catch (UserNotFoundException ex)
            {
                return false;
            }
        }

        public async Task<bool> CheckUserEmailExistsAsync(string UserEmail)
        {
            try
            {
                var data = await _indexEmail.Select<ValueTuple<string>, UserDB>
                (ValueTuple.Create(UserEmail));

                if (data.Data.Length == 1)
                {
                    return true;
                }
            }

            catch (UserException ex) { }

            return false;
        }

        public async Task<User> GetUserByEmailAsync(string userEmail)
        {
            var data = await _indexEmail.Select<ValueTuple<string>, UserDB>
                (ValueTuple.Create(userEmail));

            if (data.Data.Length != 1)
            {
                throw new UserNotFoundException();
            }

            return UserConverter.DBToBL(data.Data[0]);
        }
    }
}
