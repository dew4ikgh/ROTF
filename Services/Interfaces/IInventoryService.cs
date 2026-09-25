using ROTF.Server.Models;
using ROTF.Server.Common;

namespace ROTF.Server.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<ServiceResponse<IEnumerable<InventoryItem>>> GetPlayerInventoryAsync(int playerProgressId);
        Task<ServiceResponse<bool>> AddItemToInventoryAsync(int playerProgressId, int itemId, int quantity);
        Task<ServiceResponse<bool>> RemoveItemAsync(int id);
    }
}