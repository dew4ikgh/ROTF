using ROTF.Server.Models;
using ROTF.Server.Common;

namespace ROTF.Server.Services.Interfaces
{
    public interface IPlayerProgressService
    {
        Task<ServiceResponse<PlayerProgress>> GetProgressByIdAsync(int id);
        Task<ServiceResponse<PlayerProgress>> GetProgressByUserIdAsync(int userId);
        Task<ServiceResponse<bool>> UpdateStatsAsync(int id, float health, float stamina);
        Task<ServiceResponse<bool>> SavePositionAsync(int id, float x, float y, float z);
    }
}