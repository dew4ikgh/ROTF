using ROTF.Server.Models;
using ROTF.Server.Services.Interfaces;
using ROTF.Server.Common;
using ROTF.Server.Repositories;

namespace ROTF.Server.Services
{
    public class ItemService : IItemService
    {
        private readonly IGenericRepository<Item> _itemRepository;

        public ItemService(IGenericRepository<Item> itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<ServiceResponse<IEnumerable<Item>>> GetAllItemsAsync()
        {

            var items = await _itemRepository.GetAllAsync();
            return ServiceResponse<IEnumerable<Item>>.Ok(items, "Список усіх предметів отримано");
        }

        public async Task<ServiceResponse<Item>> GetItemByIdAsync(int id)
        {

            var item = await _itemRepository.GetByIdAsync(id);

            if (item == null)
                return ServiceResponse<Item>.Fail(ServiceResultType.NotFound, "Предмет не знайдено");

            return ServiceResponse<Item>.Ok(item, "Предмет знайдено");
        }
    }
}