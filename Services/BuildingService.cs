using ROTF.Server.Models;
using ROTF.Server.Services.Interfaces;
using ROTF.Server.DTOs;
using ROTF.Server.Common;
using ROTF.Server.Repositories; // Підключаємо репозиторії

namespace ROTF.Server.Services
{
    public class BuildingService : IBuildingService
    {
        private readonly IGenericRepository<Building> _buildingRepo;

        public BuildingService(IGenericRepository<Building> buildingRepo)
        {
            _buildingRepo = buildingRepo;
        }

        public async Task<ServiceResponse<IEnumerable<Building>>> GetBuildingsByServerAsync(int serverId)
        {
            // Використовуємо наш новий метод FindAsync
            var buildings = await _buildingRepo.FindAsync(b => b.serverid == serverId);

            return ServiceResponse<IEnumerable<Building>>.Ok(buildings, "Список споруд отримано");
        }

        public async Task<ServiceResponse<Building>> PlaceBuildingAsync(PlaceBuildingDto dto)
        {
            var newBuilding = new Building
            {
                serverid = dto.ServerId,
                ownerid = dto.OwnerId,
                buildingtype = dto.BuildingType,
                positionx = dto.PositionX,
                positiony = dto.PositionY,
                positionz = dto.PositionZ,
                health = 100.0f
            };

            await _buildingRepo.AddAsync(newBuilding);
            await _buildingRepo.SaveAsync();

            return ServiceResponse<Building>.Ok(newBuilding, "Споруду успішно побудовано!");
        }

        public async Task<ServiceResponse<bool>> DestroyBuildingAsync(int id)
        {
            var building = await _buildingRepo.GetByIdAsync(id);

            if (building == null)
                return ServiceResponse<bool>.Fail(ServiceResultType.NotFound, "Споруду не знайдено в базі даних");

            _buildingRepo.Delete(building);
            await _buildingRepo.SaveAsync();

            return ServiceResponse<bool>.Ok(true, "Споруду зруйновано");
        }
    }
}