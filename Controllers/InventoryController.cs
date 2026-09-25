using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROTF.Server.Services.Interfaces;
using ROTF.Server.Models;
using ROTF.Server.Common;

namespace ROTF.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet("player/{progressId}")]
        public async Task<IActionResult> GetInventory(int progressId)
        {
            var response = await _inventoryService.GetPlayerInventoryAsync(progressId);
            return Ok(response);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddItem([FromBody] AddItemRequest request)
        {
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

        [HttpDelete("{id}")]
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