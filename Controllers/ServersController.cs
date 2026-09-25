using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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

        /// <summary>
        /// Повертає список усіх активних ігрових серверів.
        /// </summary>
        /// <remarks>
        /// Приклад запиту:
        ///
        ///     GET /api/v1/servers
        ///
        /// Приклад відповіді (200 OK):
        ///
        ///     { "success": true, "data": [ { "id": 1, "name": "ROTF Official Ukraine" } ] }
        /// </remarks>
        /// <response code="200">Список серверів (може бути порожнім)</response>
        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetServers()
        {
            var response = await _serverService.GetAllServersAsync();
            return Ok(response);
        }
    }
}