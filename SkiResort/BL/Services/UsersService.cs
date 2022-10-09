using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.IRepositories;
using BL.Models;
using BL.Exceptions.UserExceptions;

namespace BL.Services
{
    public class UsersService
    {
        private IUsersRepository _usersRepository;
        private ICheckPermissionService _checkPermissionsService;

        public UsersService(ICheckPermissionService checkPermissionsService, IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
            _checkPermissionsService = checkPermissionsService;
        }


        public async Task<User> RegisterAsync(uint cardID, string email, string password)
        {

            if (email.Length == 0 || password.Length == 0)
            {
                throw new UserRegistrationException($"Could't register new user because of incorrect password or email");
            }


            if (await _usersRepository.CheckUserEmailExistsAsync(email))
            {
                throw new UserRegistrationException($"Could't register new user because such email already exists");
            }

            uint newUserID = await _usersRepository.AddUserAutoIncrementAsync(cardID, email, password, PermissionsEnum.AUTHORIZED);
            User authorizedUser = await _usersRepository.GetUserByEmailAsync(email);
            return authorizedUser;
        }

        public async Task<User> LogInAsync(string email, string password)
        {
            User userFromDB = await _usersRepository.GetUserByEmailAsync(email);

            if (password != userFromDB.Password)
            {
                throw new UserAuthorizationException($"Could't authorize user because of wrong password");
            }

            return userFromDB;
        }

        public async Task<List<User>> AdminGetUsersAsync(uint requesterUserID, uint offset = 0, uint limit = 0)
        {
            await _checkPermissionsService.CheckPermissionsAsync(requesterUserID);
            return await _usersRepository.GetUsersAsync(offset, limit);
        }

        public async Task<uint> AdminAddAutoIncrementUserAsync(uint requesterUserID, uint cardID, string userEmail, string password, PermissionsEnum permissions)
        {
            await _checkPermissionsService.CheckPermissionsAsync(requesterUserID);
            return await _usersRepository.AddUserAutoIncrementAsync(cardID, userEmail, password, permissions);
        }

        public async Task AdminUpdateUserAsync(uint requesterUserID, uint userID, uint newCardID, string newUserEmail, string newPassword, PermissionsEnum newPermissions)
        {
            await _checkPermissionsService.CheckPermissionsAsync(requesterUserID);
            await _usersRepository.UpdateUserByIDAsync(userID, newCardID, newUserEmail, newPassword, newPermissions);
        }

        public async Task AdminDeleteUserAsync(uint requesterUserID, uint userToDeleteID)
        {
            await _checkPermissionsService.CheckPermissionsAsync(requesterUserID);
            await _usersRepository.DeleteUserByIDAsync(userToDeleteID);
        }

        public async Task<User> AdminGetUserByIDAsync(uint requesterUserID, uint userID)
        {
            await _checkPermissionsService.CheckPermissionsAsync(requesterUserID);
            return await _usersRepository.GetUserByIdAsync(userID);
        }

    }
}
