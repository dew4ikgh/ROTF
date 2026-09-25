using ROTF.Server.Models;
using ROTF.Server.DTOs;
using ROTF.Server.Common;

namespace ROTF.Server.Services.Interfaces
{
    public interface IBuildingService
    {
        Task<ServiceResponse<IEnumerable<Building>>> GetBuildingsByServerAsync(int serverId);
        Task<ServiceResponse<Building>> PlaceBuildingAsync(PlaceBuildingDto dto);
        Task<ServiceResponse<bool>> DestroyBuildingAsync(int id);
    }
}