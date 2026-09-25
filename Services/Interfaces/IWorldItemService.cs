using ROTF.Server.Models;
using ROTF.Server.Common;

namespace ROTF.Server.Services.Interfaces
{
    public interface IWorldItemService
    {
        Task<ServiceResponse<IEnumerable<WorldItem>>> GetItemsByServerAsync(int serverId);
        Task<ServiceResponse<bool>> PickUpItemAsync(int worldItemId, int playerProgressId);
    }
}