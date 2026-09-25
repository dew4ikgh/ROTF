using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROTF.Server.Services.Interfaces;
using ROTF.Server.Common;

namespace ROTF.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WorldItemsController : ControllerBase
    {
        private readonly IWorldItemService _worldItemService;

        public WorldItemsController(IWorldItemService worldItemService)
        {
            _worldItemService = worldItemService;
        }

        [HttpGet("server/{serverId}")]
        public async Task<IActionResult> GetWorldItems(int serverId)
        {
            var response = await _worldItemService.GetItemsByServerAsync(serverId);
            return Ok(response);
        }

        [HttpPost("pickup")]
        public async Task<IActionResult> PickUpItem([FromBody] PickUpRequest request)
        {
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