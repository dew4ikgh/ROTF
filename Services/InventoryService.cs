using ROTF.Server.Models;
using ROTF.Server.Services.Interfaces;
using ROTF.Server.Common;
using ROTF.Server.Repositories;

namespace ROTF.Server.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepo;
        private readonly IGenericRepository<Item> _itemRepo;

        public InventoryService(IInventoryRepository inventoryRepo, IGenericRepository<Item> itemRepo)
        {
            _inventoryRepo = inventoryRepo;
            _itemRepo = itemRepo;
        }

        public async Task<ServiceResponse<IEnumerable<InventoryItem>>> GetPlayerInventoryAsync(int playerProgressId)
        {
            var inventory = await _inventoryRepo.GetPlayerInventoryWithDetailsAsync(playerProgressId);
            return ServiceResponse<IEnumerable<InventoryItem>>.Ok(inventory, "Інвентар гравця отримано");
        }

        public async Task<ServiceResponse<bool>> AddItemToInventoryAsync(int playerProgressId, int itemId, int quantity)
        {
            var itemExists = await _itemRepo.GetByIdAsync(itemId);
            if (itemExists == null)
                return ServiceResponse<bool>.Fail(ServiceResultType.NotFound, "Предмет не існує в базі даних");

            var playerInventory = await _inventoryRepo.GetPlayerInventoryWithDetailsAsync(playerProgressId);
            var existingSlot = playerInventory.FirstOrDefault(i => i.itemid == itemId);

            if (existingSlot != null)
            {
                existingSlot.quantity += quantity;
                _inventoryRepo.Update(existingSlot);
            }
            else
            {
                var newSlot = new InventoryItem
                {
                    playerprogressid = playerProgressId,
                    itemid = itemId,
                    quantity = quantity
                };
                await _inventoryRepo.AddAsync(newSlot);
            }

            await _inventoryRepo.SaveAsync();
            return ServiceResponse<bool>.Ok(true, "Предмет додано до інвентаря");
        }

        public async Task<ServiceResponse<bool>> RemoveItemAsync(int id)
        {
            var item = await _inventoryRepo.GetByIdAsync(id);
            if (item == null)
                return ServiceResponse<bool>.Fail(ServiceResultType.NotFound, "Предмет не знайдено в інвентарі");

            _inventoryRepo.Delete(item);
            await _inventoryRepo.SaveAsync();

            return ServiceResponse<bool>.Ok(true, "Предмет успішно видалено з інвентаря");
        }
    }
}