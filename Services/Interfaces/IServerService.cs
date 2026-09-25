using ROTF.Server.Models;
using ROTF.Server.Common;

namespace ROTF.Server.Services.Interfaces
{
    public interface IServerService
    {
        Task<ServiceResponse<IEnumerable<GameServer>>> GetAllServersAsync();
    }
}