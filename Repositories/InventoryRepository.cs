using Microsoft.EntityFrameworkCore;
using ROTF.Server.Data;
using ROTF.Server.Models;

namespace ROTF.Server.Repositories
{
    public class InventoryRepository : GenericRepository<InventoryItem>, IInventoryRepository
    {
        public InventoryRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<InventoryItem>> GetPlayerInventoryWithDetailsAsync(int playerProgressId)
        {
            return await _dbSet
                .Where(i => i.playerprogressid == playerProgressId)
                .Include(i => i.item)
                .ToListAsync();
        }
    }
}