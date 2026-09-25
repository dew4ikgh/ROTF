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
    public class ServersController : ControllerBase
    {
        private readonly IServerService _serverService;

        public ServersController(IServerService serverService)
        {
            _serverService = serverService;
        }

        [HttpGet]
        public async Task<IActionResult> GetServers()
        {
            var response = await _serverService.GetAllServersAsync();
            return Ok(response);
        }
    }
}