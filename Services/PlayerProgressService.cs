using ROTF.Server.Models;
using ROTF.Server.Services.Interfaces;
using ROTF.Server.Common;
using ROTF.Server.Repositories;

namespace ROTF.Server.Services
{
    public class PlayerProgressService : IPlayerProgressService
    {
        private readonly IGenericRepository<PlayerProgress> _playerRepo;

        public PlayerProgressService(IGenericRepository<PlayerProgress> playerRepo)
        {
            _playerRepo = playerRepo;
        }

        public async Task<ServiceResponse<PlayerProgress>> GetProgressByIdAsync(int id)
        {
            var progress = await _playerRepo.GetByIdAsync(id);
            if (progress == null)
                return ServiceResponse<PlayerProgress>.Fail(ServiceResultType.NotFound, "Прогрес не знайдено");

            return ServiceResponse<PlayerProgress>.Ok(progress, "Прогрес завантажено");
        }

        public async Task<ServiceResponse<PlayerProgress>> GetProgressByUserIdAsync(int userId)
        {
            // Шукаємо першого гравця, який підходить під умову
            var players = await _playerRepo.FindAsync(p => p.userid == userId);
            var progress = players.FirstOrDefault();

            if (progress == null)
                return ServiceResponse<PlayerProgress>.Fail(ServiceResultType.NotFound, "Для цього користувача не знайдено записів прогресу");

            return ServiceResponse<PlayerProgress>.Ok(progress, "Прогрес користувача отримано");
        }

        public async Task<ServiceResponse<bool>> UpdateStatsAsync(int id, float health, float stamina)
        {
            var progress = await _playerRepo.GetByIdAsync(id);
            if (progress == null)
                return ServiceResponse<bool>.Fail(ServiceResultType.NotFound, "Неможливо оновити: прогрес не знайдено");

            progress.health = health;
            progress.stamina = stamina;

            _playerRepo.Update(progress);
            await _playerRepo.SaveAsync();
            return ServiceResponse<bool>.Ok(true, "Характеристики успішно оновлено");
        }

        public async Task<ServiceResponse<bool>> SavePositionAsync(int id, float x, float y, float z)
        {
            var progress = await _playerRepo.GetByIdAsync(id);
            if (progress == null)
                return ServiceResponse<bool>.Fail(ServiceResultType.NotFound, "Неможливо зберегти позицію: прогрес не знайдено");

            progress.positionx = x;
            progress.positiony = y;
            progress.positionz = z;

            _playerRepo.Update(progress);
            await _playerRepo.SaveAsync();
            return ServiceResponse<bool>.Ok(true, "Позицію гравця збережено");
        }
    }
}