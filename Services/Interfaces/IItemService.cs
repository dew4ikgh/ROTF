using ROTF.Server.Models;
using ROTF.Server.Common;

namespace ROTF.Server.Services.Interfaces
{
    public interface IItemService
    {
        Task<ServiceResponse<IEnumerable<Item>>> GetAllItemsAsync();
        Task<ServiceResponse<Item>> GetItemByIdAsync(int id);
    }
}