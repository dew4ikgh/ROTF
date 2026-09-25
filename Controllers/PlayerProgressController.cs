using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROTF.Server.Services.Interfaces;
using ROTF.Server.Common;

namespace ROTF.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PlayerProgressController : ControllerBase
    {
        private readonly IPlayerProgressService _playerService;

        public PlayerProgressController(IPlayerProgressService playerService)
        {
            _playerService = playerService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProgress(int id)
        {
            var response = await _playerService.GetProgressByIdAsync(id);
            if (!response.Success) return NotFound(response);

            return Ok(response);
        }

        [HttpPost("update-stats")]
        public async Task<IActionResult> UpdateStats([FromBody] UpdateStatsRequest request)
        {
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

        [HttpPost("save-position")]
        public async Task<IActionResult> SavePosition([FromBody] SavePositionRequest request)
        {
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