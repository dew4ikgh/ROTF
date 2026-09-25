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
    public class WorldItemsController : ControllerBase
    {
        private readonly IWorldItemService _worldItemService;
        private readonly IGenericRepository<PlayerProgress> _playerProgressRepo;

        public WorldItemsController(
            IWorldItemService worldItemService,
            IGenericRepository<PlayerProgress> playerProgressRepo)
        {
            _worldItemService = worldItemService;
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

        /// <summary>
        /// Повертає список предметів, розкиданих або заспавнених у світі конкретного сервера.
        /// </summary>
        /// <remarks>
        /// Приклад запиту:
        ///
        ///     GET /api/v1/worlditems/server/1
        ///
        /// Приклад відповіді (200 OK):
        ///
        ///     { "success": true, "data": [ { "id": 9, "itemId": 2, "positionX": 50.0 } ] }
        /// </remarks>
        /// <param name="serverId">Ідентифікатор ігрового сервера</param>
        /// <response code="200">Список предметів у світі (може бути порожнім)</response>
        [HttpGet("server/{serverId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWorldItems(int serverId)
        {
            var response = await _worldItemService.GetItemsByServerAsync(serverId);
            return Ok(response);
        }

        /// <summary>
        /// Підбирає предмет зі світу в інвентар персонажа поточного користувача.
        /// </summary>
        /// <remarks>
        /// Приклад запиту:
        ///
        ///     POST /api/v1/worlditems/pickup
        ///     { "worldItemId": 9, "playerProgressId": 1 }
        /// </remarks>
        /// <param name="request">Id предмета у світі та id прогресу, до чийого інвентарю додати</param>
        /// <response code="200">Предмет підібрано, доданий в інвентар</response>
        /// <response code="403">playerProgressId у запиті належить іншому користувачу</response>
        /// <response code="404">Предмет із таким worldItemId вже підібрано або не існує</response>
        [HttpPost("pickup")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PickUpItem([FromBody] PickUpRequest request)
        {
            int userId = GetCurrentUserId();

            var progress = (await _playerProgressRepo.FindAsync(
                p => p.id == request.PlayerProgressId)).FirstOrDefault();

            if (progress == null || progress.userid != userId)
                return Forbid();

            var response = await _worldItemService.PickUpItemAsync(request.WorldItemId, request.PlayerProgressId);

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

    public class PickUpRequest
    {
        public int WorldItemId { get; set; }
        public int PlayerProgressId { get; set; }
    }
}