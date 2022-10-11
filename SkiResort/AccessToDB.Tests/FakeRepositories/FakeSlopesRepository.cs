using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using BL;
using BL.Models;
using BL.IRepositories;



namespace AccessToDB.Tests.FakeRepositories
{
    public class FakeSlopesRepository : ISlopesRepository
    {
        private static readonly List<Slope> data = new();

        public async Task AddSlopeAsync(uint slopeID, string slopeName, bool isOpen, uint difficultyLevel)
        {
            if (await CheckSlopeIdExistsAsync(slopeID))
            {
                throw new Exception();
            }
            data.Add(new Slope(slopeID, slopeName, isOpen, difficultyLevel));
        }

        public async Task<uint> AddSlopeAutoIncrementAsync(string slopeName, bool isOpen, uint difficultyLevel)
        {
            uint maxSlopeID = 0;
            foreach (var slopeFromDB in data)
            {
                if (slopeFromDB.SlopeID > maxSlopeID)
                    maxSlopeID = slopeFromDB.SlopeID;
            }
            Slope slopeWithCorrectId = new(maxSlopeID + 1, slopeName, isOpen, difficultyLevel);
            await AddSlopeAsync(slopeWithCorrectId.SlopeID, slopeWithCorrectId.SlopeName, slopeWithCorrectId.IsOpen, slopeWithCorrectId.DifficultyLevel);
            return slopeWithCorrectId.SlopeID;
        }

        public async Task<bool> CheckSlopeIdExistsAsync(uint slopeID)
        {
            foreach (var slope in data)
            {
                if (slope.SlopeID == slopeID)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task DeleteSlopeByIDAsync(uint slopeID)
        {
            foreach (var obj in data)
            {
                if (obj.SlopeID == slopeID)
                {
                    data.Remove(obj);
                    return;
                }
            }
            throw new Exception();
        }

        public async Task<Slope> GetSlopeByIdAsync(uint slopeID)
        {
            foreach (var slope in data)
            {
                if (slope.SlopeID == slopeID)
                    return slope;
            }
            throw new Exception();
        }

        public async Task<Slope> GetSlopeByNameAsync(string name)
        {
            foreach (var slope in data)
            {
                if (slope.SlopeName == name)
                    return slope;
            }
            throw new Exception();
        }
        public async Task<List<Slope>> GetSlopesAsync(uint offset = 0, uint limit = 0)
        {
            if (limit != 0)
                return data.GetRange((int)offset, (int)limit);
            else
                return data.GetRange((int)offset, (int)data.Count);
        }

        public async Task UpdateSlopeByIDAsync(uint slopeID, string newSlopeName, bool newIsOpen, uint newDifficultyLevel)
        {
            for (int i = 0; i < data.Count; i++)
            {
                Slope slopeFromDB = data[i];
                if (slopeFromDB.SlopeID == slopeID)
                {
                    data.Remove(slopeFromDB);
                    data.Insert(i, new Slope(slopeID, newSlopeName, newIsOpen, newDifficultyLevel));
                    return;
                }
            }
            throw new Exception();
        }
    }
}
