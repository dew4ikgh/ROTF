using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ROTF.Server.Services.Interfaces;
using ROTF.Server.Common;
using ROTF.Server.Models;
using ROTF.Server.Repositories;
using System.Linq;

namespace ROTF.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PlayerProgressController : ControllerBase
    {
        private readonly IPlayerProgressService _playerService;
        private readonly IGenericRepository<PlayerProgress> _playerProgressRepo;

        public PlayerProgressController(
            IPlayerProgressService playerService,
            IGenericRepository<PlayerProgress> playerProgressRepo)
        {
            _playerService = playerService;
            _playerProgressRepo = playerProgressRepo;
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(claim) || !int.TryParse(claim, out int userId))
            {
                throw new UnauthorizedAccessException("Користувач не авторизований або токен пошкоджено.");
            }
            return userId;
        }

        private async Task<bool> IsOwner(int progressId, int userId)
        {
            var progress = (await _playerProgressRepo.FindAsync(p => p.id == progressId)).FirstOrDefault();
            return progress != null && progress.userid == userId;
        }

        /// <summary>
        /// Повертає поточний ігровий прогрес (здоров'я, стаміна, позиція тощо) за id.
        /// </summary>
        /// <remarks>
        /// Приклад запиту:
        ///
        ///     GET /api/v1/playerprogress/1
        ///
        /// Приклад відповіді (200 OK):
        ///
        ///     { "success": true, "data": { "id": 1, "health": 87.5, "stamina": 60.0 } }
        /// </remarks>
        /// <param name="id">Ідентифікатор запису прогресу</param>
        /// <response code="200">Дані прогресу знайдено</response>
        /// <response code="403">Запис id належить іншому користувачу</response>
        /// <response code="404">Запис із таким id не знайдено</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProgress(int id)
        {
            int userId = GetCurrentUserId();
            if (!await IsOwner(id, userId))
                return Forbid();

            var response = await _playerService.GetProgressByIdAsync(id);
            if (!response.Success) return NotFound(response);

            return Ok(response);
        }

        /// <summary>
        /// Оновлює показники виживання персонажа (здоров'я, стаміну).
        /// </summary>
        /// <remarks>
        /// Приклад запиту:
        ///
        ///     POST /api/v1/playerprogress/update-stats
        ///     { "id": 1, "health": 75.0, "stamina": 40.0 }
        /// </remarks>
        /// <param name="request">id прогресу та нові значення health/stamina</param>
        /// <response code="200">Показники оновлено</response>
        /// <response code="403">id прогресу належить іншому користувачу</response>
        /// <response code="404">Запис прогресу не знайдено</response>
        [HttpPost("update-stats")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStats([FromBody] UpdateStatsRequest request)
        {
            int userId = GetCurrentUserId();
            if (!await IsOwner(request.Id, userId))
                return Forbid();

            var response = await _playerService.UpdateStatsAsync(request.Id, request.Health, request.Stamina);

            if (!response.Success)
            {
                return response.ResultType switch
                {
                    ServiceResultType.NotFound => NotFound(response),
                    _ => BadRequest(response)
                };
            }

            return Ok(response);
        }

        /// <summary>
        /// Зберігає поточні тривимірні координати персонажа у світі.
        /// </summary>
        /// <remarks>
        /// Приклад запиту:
        ///
        ///     POST /api/v1/playerprogress/save-position
        ///     { "id": 1, "x": 10.5, "y": 0.0, "z": -25.3 }
        /// </remarks>
        /// <param name="request">id прогресу та координати X/Y/Z</param>
        /// <response code="200">Позицію збережено</response>
        /// <response code="403">id прогресу належить іншому користувачу</response>
        /// <response code="404">Запис прогресу не знайдено</response>
        [HttpPost("save-position")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SavePosition([FromBody] SavePositionRequest request)
        {
            int userId = GetCurrentUserId();
            if (!await IsOwner(request.Id, userId))
                return Forbid();

            var response = await _playerService.SavePositionAsync(request.Id, request.X, request.Y, request.Z);

            if (!response.Success)
            {
                return response.ResultType switch
                {
                    ServiceResultType.NotFound => NotFound(response),
                    _ => BadRequest(response)
                };
            }

            return Ok(response);
        }
    }

    public class UpdateStatsRequest
    {
        public int Id { get; set; }
        public float Health { get; set; }
        public float Stamina { get; set; }
    }

    public class SavePositionRequest
    {
        public int Id { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }
}