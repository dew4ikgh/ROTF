using ROTF.Server.Models;
using ROTF.Server.Services.Interfaces;
using ROTF.Server.Common;
using ROTF.Server.Repositories;

namespace ROTF.Server.Services
{
    public class WorldItemService : IWorldItemService
    {
        private readonly IGenericRepository<WorldItem> _worldItemRepo;
        private readonly IInventoryService _inventoryService;

        public WorldItemService(IGenericRepository<WorldItem> worldItemRepo, IInventoryService inventoryService)
        {
            _worldItemRepo = worldItemRepo;
            _inventoryService = inventoryService;
        }

        public async Task<ServiceResponse<IEnumerable<WorldItem>>> GetItemsByServerAsync(int serverId)
        {
            // Передаємо умову пошуку і вказуємо, що треба завантажити зв'язану модель Item!
            var items = await _worldItemRepo.FindAsync(
                wi => wi.serverid == serverId,
                wi => wi.item!
            );

            return ServiceResponse<IEnumerable<WorldItem>>.Ok(items, "Предмети світу завантажено");
        }

        public async Task<ServiceResponse<bool>> PickUpItemAsync(int worldItemId, int playerProgressId)
        {
            var worldItem = await _worldItemRepo.GetByIdAsync(worldItemId);

            if (worldItem == null)
                return ServiceResponse<bool>.Fail(ServiceResultType.NotFound, "Цей предмет уже хтось підняв або його не існує");

            var inventoryResult = await _inventoryService.AddItemToInventoryAsync(playerProgressId, worldItem.itemid, worldItem.quantity);

            if (!inventoryResult.Success)
                return ServiceResponse<bool>.Fail(inventoryResult.ResultType, "Не вдалося підняти предмет: " + inventoryResult.Message);

            _worldItemRepo.Delete(worldItem);
            await _worldItemRepo.SaveAsync();

            return ServiceResponse<bool>.Ok(true, "Предмет успішно додано до вашого інвентарю");
        }
    }
}