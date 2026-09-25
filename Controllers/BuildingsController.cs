using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("server/{serverId}")]
        public async Task<IActionResult> GetServerBuildings(int serverId)
        {
            var response = await _buildingService.GetBuildingsByServerAsync(serverId);
            return Ok(response);
        }

        [HttpPost("place")]
        public async Task<IActionResult> PlaceBuilding([FromBody] PlaceBuildingDto dto)
        {
            var response = await _buildingService.PlaceBuildingAsync(dto);
            // Тут завжди Success, бо це створення, але для структури повертаємо response
            return Ok(response);
        }

        [HttpDelete("{id}")]
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