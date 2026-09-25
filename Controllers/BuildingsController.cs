using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ROTF.Server.Models;
using ROTF.Server.Services.Interfaces;
using ROTF.Server.DTOs;
using ROTF.Server.Common;

namespace ROTF.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BuildingsController : ControllerBase
    {
        private readonly IBuildingService _buildingService;

        public BuildingsController(IBuildingService buildingService)
        {
            _buildingService = buildingService;
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
        /// Повертає список усіх споруд, зведених гравцями на конкретному сервері.
        /// </summary>
        /// <remarks>
        /// Приклад запиту:
        ///
        ///     GET /api/v1/buildings/server/1
        ///
        /// Приклад відповіді (200 OK):
        ///
        ///     {
        ///        "success": true,
        ///        "data": [
        ///           { "id": 5, "buildingType": "Wood Wall", "positionX": 15.0, "health": 100.0 }
        ///        ]
        ///     }
        /// </remarks>
        /// <param name="serverId">Ідентифікатор ігрового сервера</param>
        /// <response code="200">Список споруд на сервері (може бути порожнім)</response>
        [HttpGet("server/{serverId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetServerBuildings(int serverId)
        {
            var response = await _buildingService.GetBuildingsByServerAsync(serverId);
            return Ok(response);
        }

        /// <summary>
        /// Розміщує нову споруду у світі від імені поточного авторизованого гравця.
        /// </summary>
        /// <remarks>
        /// Власника (OwnerId) сервер бере з JWT-токена, а не з тіла запиту —
        /// це навмисно, щоб гравець не міг "побудувати" щось від чужого імені.
        ///
        /// Приклад запиту:
        ///
        ///     POST /api/v1/buildings/place
        ///     {
        ///        "serverId": 1,
        ///        "buildingType": "Wood Wall",
        ///        "positionX": 15.0, "positionY": 0.0, "positionZ": -30.0
        ///     }
        ///
        /// Приклад відповіді (200 OK):
        ///
        ///     { "success": true, "data": { "id": 6, "buildingType": "Wood Wall" } }
        /// </remarks>
        /// <param name="dto">Тип споруди, сервер та координати розміщення</param>
        /// <response code="200">Споруду успішно розміщено</response>
        /// <response code="400">Помилка валідації (наприклад, невірний тип споруди)</response>
        [HttpPost("place")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PlaceBuilding([FromBody] PlaceBuildingDto dto)
        {
            dto.OwnerId = GetCurrentUserId();

            var response = await _buildingService.PlaceBuildingAsync(dto);

            if (!response.Success)
            {
                return response.ResultType switch
                {
                    ServiceResultType.ValidationError => BadRequest(new { message = response.Message }),
                    _ => BadRequest(new { message = response.Message })
                };
            }

            return Ok(response);
        }


/// <summary>
        /// Знищує (видаляє) споруду за її ідентифікатором.
        /// </summary>
        /// <remarks>
        /// Приклад запиту:
        ///
        ///     DELETE /api/v1/buildings/6
        ///
        /// Приклад відповіді при відсутності споруди (404 Not Found):
        ///
        ///     { "message": "Споруду не знайдено" }
        /// </remarks>
        /// <param name="id">Ідентифікатор споруди, яку потрібно знести</param>
        /// <response code="200">Споруду успішно знищено</response>
        /// <response code="404">Споруду з таким id не знайдено</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DestroyBuilding(int id)
        {
            var response = await _buildingService.DestroyBuildingAsync(id);

            if (!response.Success)
            {
                return response.ResultType switch
                {
                    ServiceResultType.NotFound => NotFound(new { message = response.Message }),
                    _ => BadRequest(new { message = response.Message })
                };
            }

            return Ok(response);
        }
    }
}