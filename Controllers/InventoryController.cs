using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ROTF.Server.Services.Interfaces;
using ROTF.Server.Models;
using ROTF.Server.Common;
using ROTF.Server.Repositories;
using System.Linq;

namespace ROTF.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly IGenericRepository<PlayerProgress> _playerProgressRepo;

        public InventoryController(
            IInventoryService inventoryService,
            IGenericRepository<PlayerProgress> playerProgressRepo)
        {
            _inventoryService = inventoryService;
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

        private async Task<bool> IsOwnerOfProgress(int progressId, int userId)
        {
            var progress = (await _playerProgressRepo.FindAsync(p => p.id == progressId)).FirstOrDefault();
            return progress != null && progress.userid == userId;
        }

        /// <summary>
        /// Повертає вміст інвентарю конкретного ігрового прогресу (персонажа).
        /// </summary>
        /// <remarks>
        /// Приклад запиту:
        ///
        ///     GET /api/v1/inventory/player/1
        ///
        /// Приклад відповіді (200 OK):
        ///
        ///     { "success": true, "data": [ { "itemId": 1, "name": "Stone Axe", "quantity": 1 } ] }
        /// </remarks>
        /// <param name="progressId">Ідентифікатор запису прогресу (персонажа), чий інвентар запитується</param>
        /// <response code="200">Вміст інвентарю</response>
        /// <response code="403">Запитаний progressId належить іншому користувачу</response>
        [HttpGet("player/{progressId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetInventory(int progressId)
        {
            int userId = GetCurrentUserId();
            if (!await IsOwnerOfProgress(progressId, userId))
                return Forbid();

            var response = await _inventoryService.GetPlayerInventoryAsync(progressId);
            return Ok(response);
        }

        /// <summary>
        /// Додає вказану кількість предмета в інвентар персонажа.
        /// </summary>
        /// <remarks>
        /// Приклад запиту:
        ///
        ///     POST /api/v1/inventory/add
        ///     { "playerProgressId": 1, "itemId": 3, "quantity": 5 }
        ///
        /// Приклад відповіді (200 OK):
        ///
        ///     { "success": true, "data": { "itemId": 3, "quantity": 25 } }
        /// </remarks>
        /// <param name="request">progressId персонажа, id предмета та кількість</param>
        /// <response code="200">Предмет успішно додано</response>
        /// <response code="403">progressId у запиті належить іншому користувачу</response>
        /// <response code="404">Предмет із таким itemId не знайдено в довіднику</response>
        [HttpPost("add")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddItem([FromBody] AddItemRequest request)
        {
            int userId = GetCurrentUserId();
            if (!await IsOwnerOfProgress(request.PlayerProgressId, userId))
                return Forbid();

            var response = await _inventoryService.AddItemToInventoryAsync(
                request.PlayerProgressId, request.ItemId, request.Quantity);

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
        /// Видаляє слот інвентарю за його ідентифікатором.
        /// </summary>
        /// <remarks>
        /// Приклад запиту:
        ///
        ///     DELETE /api/v1/inventory/12
        /// </remarks>
        /// <param name="id">Ідентифікатор слоту інвентарю (не itemId!)</param>
        /// <response code="200">Слот успішно видалено</response>
        /// <response code="404">Слот із таким id не знайдено</response>
        /// <remarks>
        /// TODO: наразі не перевіряється, що слот належить поточному користувачу
        /// (окрема задача — сервіс має повертати playerprogressid видаленого запису).
        /// </remarks>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var response = await _inventoryService.RemoveItemAsync(id);

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

    public class AddItemRequest
    {
        public int PlayerProgressId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }
}