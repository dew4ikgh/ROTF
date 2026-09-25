using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ROTF.Server.Services.Interfaces;
using ROTF.Server.Common;
using ROTF.Server.Models;

namespace ROTF.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        /// <summary>
        /// Повертає повний довідник усіх ігрових предметів.
        /// </summary>
        /// <remarks>
        /// Публічний довідник — тут немає "чужих" даних, тому перевірка власника не потрібна.
        ///
        /// Приклад запиту:
        ///
        ///     GET /api/v1/items
        /// </remarks>
        /// <response code="200">Повний список предметів</response>
        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllItems()
        {
            var response = await _itemService.GetAllItemsAsync();
            return Ok(response);
        }

        /// <summary>
        /// Повертає опис одного предмета за його ідентифікатором.
        /// </summary>
        /// <remarks>
        /// Приклад запиту:
        ///
        ///     GET /api/v1/items/1
        /// </remarks>
        /// <param name="id">Ідентифікатор предмета в довіднику</param>
        /// <response code="200">Опис предмета знайдено</response>
        /// <response code="404">Предмета з таким id не існує</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetItem(int id)
        {
            var response = await _itemService.GetItemByIdAsync(id);

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
}