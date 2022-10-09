using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.IRepositories;
using BL.Models;

namespace BL.Services
{
    public class SlopesService
    {
        private ISlopesRepository _slopesRepository;
        private ILiftsSlopesRepository _liftsSlopesRepository;
        private ICheckPermissionService _checkPermissionsService;

        public SlopesService(ICheckPermissionService checkPermissionsService ,ISlopesRepository slopesRepository, ILiftsSlopesRepository liftsSlopesRepository)
        {
            _checkPermissionsService = checkPermissionsService;
            _slopesRepository = slopesRepository;
            _liftsSlopesRepository = liftsSlopesRepository;
        }

        public async Task<Slope> GetSlopeInfoAsync(uint requesterUserID, string SlopeName)
        {
            await _checkPermissionsService.CheckPermissionsAsync(requesterUserID);
            Slope slope = await _slopesRepository.GetSlopeByNameAsync(SlopeName);
            slope = new(slope, await _liftsSlopesRepository.GetLiftsBySlopeIdAsync(slope.SlopeID));

            return slope;

        }

        public async Task<List<Slope>> GetSlopesInfoAsync(uint requesterUserID, uint offset = 0, uint limit = 0)
        {
            await _checkPermissionsService.CheckPermissionsAsync(requesterUserID);
            List<Slope> slopes = await _slopesRepository.GetSlopesAsync(offset, limit);
            List<Slope> slopesFull = new();

            foreach (Slope slope in slopes)
            {
                slopesFull.Add(new Slope(slope, await _liftsSlopesRepository.GetLiftsBySlopeIdAsync(slope.SlopeID)));
            }
            return slopesFull;
        }

        public async Task UpdateSlopeInfoAsync(uint requesterUserID, string slopeName, bool newIsOpen, uint newDifficultyLevel)
        {
            await _checkPermissionsService.CheckPermissionsAsync(requesterUserID);
            uint slopeID = (await _slopesRepository.GetSlopeByNameAsync(slopeName)).SlopeID;
            await _slopesRepository.UpdateSlopeByIDAsync(slopeID, slopeName, newIsOpen, newDifficultyLevel);
        }

        public async Task AdminDeleteSlopeAsync(uint requesterUserID, string slopeName)
        {
            await _checkPermissionsService.CheckPermissionsAsync(requesterUserID);
            Slope slope = await _slopesRepository.GetSlopeByNameAsync(slopeName);
            List<LiftSlope> lifts_slopes = await _liftsSlopesRepository.GetLiftsSlopesAsync();
            foreach (LiftSlope lift_slope in lifts_slopes)
            {
                if (lift_slope.SlopeID == slope.SlopeID)
                {
                    await _liftsSlopesRepository.DeleteLiftSlopesByIDAsync(lift_slope.RecordID);
                }
            }
            await _slopesRepository.DeleteSlopeByIDAsync(slope.SlopeID);
        }


        public async Task<uint> AdminAddAutoIncrementSlopeAsync(uint requesterUserID, string slopeName, bool isOpen, uint difficultyLevel)
        {
            await _checkPermissionsService.CheckPermissionsAsync(requesterUserID);
            return await _slopesRepository.AddSlopeAutoIncrementAsync(slopeName, isOpen, difficultyLevel);
        }

        public async Task AdminAddSlopeAsync(uint requesterUserID, uint slopeID, string slopeName, bool isOpen, uint difficultyLevel)
        {
            await _checkPermissionsService.CheckPermissionsAsync(requesterUserID);
            await _slopesRepository.AddSlopeAsync(slopeID, slopeName, isOpen, difficultyLevel);
        }


    }
}
