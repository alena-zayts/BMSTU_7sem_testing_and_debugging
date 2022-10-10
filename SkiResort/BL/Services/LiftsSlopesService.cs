using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.IRepositories;
using BL.Models;

namespace BL.Services
{
    public class LiftsSlopesService
    {
        private ILiftsSlopesRepository _liftsSlopesRepository;
        private ILiftsRepository _liftsRepository;
        private ISlopesRepository _slopesRepository;
        private ICheckPermissionService _checkPermissionsService;

        public LiftsSlopesService(ICheckPermissionService checkPermissionsService, ILiftsSlopesRepository liftsSlopesRepository, 
            ILiftsRepository liftsRepository, ISlopesRepository slopesRepository)
        {
            _liftsSlopesRepository = liftsSlopesRepository;
            _liftsRepository = liftsRepository;
            _slopesRepository = slopesRepository;
            _checkPermissionsService = checkPermissionsService;
        }

        public async Task<List<LiftSlope>> GetLiftsSlopesInfoAsync(uint requesterUserID, uint offset = 0, uint limit = 0)
        {
            await _checkPermissionsService.CheckPermissionsAsync(requesterUserID);
            return await _liftsSlopesRepository.GetLiftsSlopesAsync(offset, limit);
        }

        public async Task AdminDeleteLiftSlopeAsync(uint requesterUserID, string liftName, string slopeName)
        {
            await _checkPermissionsService.CheckPermissionsAsync(requesterUserID);

            Lift lift = await _liftsRepository.GetLiftByNameAsync(liftName);
            Slope slope = await _slopesRepository.GetSlopeByNameAsync(slopeName);
            await _liftsSlopesRepository.DeleteLiftSlopesByIDsAsync(lift.LiftID, slope.SlopeID);
        }

        public async Task<uint> AdminAddAutoIncrementLiftSlopeAsync(uint requesterUserID, string liftName, string slopeName)
        {
            await _checkPermissionsService.CheckPermissionsAsync(requesterUserID);
            Lift lift = await _liftsRepository.GetLiftByNameAsync(liftName);
            Slope slope = await _slopesRepository.GetSlopeByNameAsync(slopeName);
            return await _liftsSlopesRepository.AddLiftSlopeAutoIncrementAsync(lift.LiftID, slope.SlopeID);
        }
    }
}
