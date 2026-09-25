using ROTF.Server.Models;

namespace ROTF.Server.Repositories
{
    public interface IInventoryRepository : IGenericRepository<InventoryItem>
    {
        Task<IEnumerable<InventoryItem>> GetPlayerInventoryWithDetailsAsync(int playerProgressId);
    }
}