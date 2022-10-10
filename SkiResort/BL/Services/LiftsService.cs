using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.IRepositories;
using BL.Models;
using BL.Exceptions.LiftExceptions;

namespace BL.Services
{
    public class LiftsService
    {
        private ILiftsRepository _liftsRepository;
        private ILiftsSlopesRepository _liftsSlopesRepository;
        private ITurnstilesRepository _turnstilesRepository;
        private ICheckPermissionService _checkPermissionsService;

        public LiftsService(ICheckPermissionService checkPermissionsService, ILiftsRepository liftsRepository, IUsersRepository usersRepository, ILiftsSlopesRepository liftsSlopesRepository, ITurnstilesRepository turnstilesRepository)
        {
            _liftsRepository = liftsRepository;
            _liftsSlopesRepository = liftsSlopesRepository;
            _turnstilesRepository = turnstilesRepository;
            _checkPermissionsService = checkPermissionsService;
        }

        public async Task<Lift> GetLiftInfoAsync(uint requesterUserID, string LiftName)
        {
            await _checkPermissionsService.CheckPermissionsAsync(requesterUserID);

            Lift lift = await _liftsRepository.GetLiftByNameAsync(LiftName);
            Lift liftFull = new(lift, await _liftsSlopesRepository.GetSlopesByLiftIdAsync(lift.LiftID));

            return liftFull;

        }

        public async Task<List<Lift>> GetLiftsInfoAsync(uint requesterUserID, uint offset = 0, uint limit = 0)
        {
            await _checkPermissionsService.CheckPermissionsAsync(requesterUserID);

            List<Lift> lifts = await _liftsRepository.GetLiftsAsync(offset, limit);
            List<Lift> liftsFull = new();

            foreach (Lift lift in lifts)
            {
                liftsFull.Add(new(lift, await _liftsSlopesRepository.GetSlopesByLiftIdAsync(lift.LiftID)));

            }
            return liftsFull;
        }

        public async Task UpdateLiftInfoAsync(uint requesterUserID, string liftName, bool isOpen, uint seatsAmount, uint liftingTime)
        {
            await _checkPermissionsService.CheckPermissionsAsync(requesterUserID);

            uint liftID = (await _liftsRepository.GetLiftByNameAsync(liftName)).LiftID;
            await _liftsRepository.UpdateLiftByIDAsync(liftID, liftName, isOpen, seatsAmount, liftingTime);
        }

        public async Task AdminDeleteLiftAsync(uint requesterUserID, string liftName)
        {
            await _checkPermissionsService.CheckPermissionsAsync(requesterUserID);

            Lift lift = await _liftsRepository.GetLiftByNameAsync(liftName);
            List<Turnstile> connected_turnstiles = await _turnstilesRepository.GetTurnstilesByLiftIdAsync(lift.LiftID);
            if (connected_turnstiles == null)
            {
                throw new LiftDeleteException("Cannot delete lift because it has connected turnstiles");
            }

            List<LiftSlope> lift_slopes = await _liftsSlopesRepository.GetLiftsSlopesAsync();
            foreach (LiftSlope lift_slope in lift_slopes)
            {
                if (lift_slope.LiftID == lift.LiftID)
                {
                    await _liftsSlopesRepository.DeleteLiftSlopesByIDAsync(lift_slope.RecordID);
                }
            }


            await _liftsRepository.DeleteLiftByIDAsync(lift.LiftID);
        }


        public async Task<uint> AdminAddAutoIncrementLiftAsync(uint requesterUserID, string liftName, bool isOpen, uint seatsAmount, uint liftingTime)
        {
            await _checkPermissionsService.CheckPermissionsAsync(requesterUserID);
            return await _liftsRepository.AddLiftAutoIncrementAsync(liftName, isOpen, seatsAmount, liftingTime);
        }

    }
}
