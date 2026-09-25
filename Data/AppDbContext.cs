using Microsoft.EntityFrameworkCore;
using ROTF.Server.Models;

namespace ROTF.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<PlayerProgress> PlayerProgress { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<InventoryItem> Inventory { get; set; }
        public DbSet<GameServer> Servers { get; set; }
        public DbSet<WorldItem> world_items { get; set; }

    }
}