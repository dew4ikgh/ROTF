using ROTF.Server.Models;
using ROTF.Server.Services.Interfaces;
using ROTF.Server.Common;
using ROTF.Server.Repositories;

namespace ROTF.Server.Services
{
    public class ServerService : IServerService
    {
        private readonly IGenericRepository<GameServer> _serverRepo;

        public ServerService(IGenericRepository<GameServer> serverRepo)
        {
            _serverRepo = serverRepo;
        }

        public async Task<ServiceResponse<IEnumerable<GameServer>>> GetAllServersAsync()
        {
            throw new Exception("Тестовий збій бази даних для перевірки логів!");
            var servers = await _serverRepo.GetAllAsync();

            return ServiceResponse<IEnumerable<GameServer>>.Ok(servers, "Список ігрових серверів отримано");
        }
    }
}