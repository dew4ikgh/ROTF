using Microsoft.AspNetCore.Mvc;
using ROTF.Server.Services.Interfaces;
using ROTF.Api.Dtos;
using ROTF.Server.Common;

namespace ROTF.Server.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var response = await _authService.RegisterAsync(dto);

            if (!response.Success)
            {
                // Використовуємо switch для повернення правильного статус-коду
                return response.ResultType switch
                {
                    ServiceResultType.Conflict => Conflict(new { message = response.Message }),
                    ServiceResultType.ValidationError => BadRequest(new { message = response.Message }),
                    _ => BadRequest(new { message = response.Message })
                };
            }

            return Ok(new { message = response.Message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var response = await _authService.LoginAsync(dto);

            if (!response.Success)
            {
                return response.ResultType switch
                {
                    ServiceResultType.Unauthorized => Unauthorized(new { message = response.Message }),
                    _ => BadRequest(new { message = response.Message })
                };
            }

            // Оскільки логін успішний, дані лежать у response.Data
            return Ok(new
            {
                message = response.Message,
                token = response.Data?.Token,
                id = response.Data?.User.id,
                username = response.Data?.User.username,
                role = response.Data?.User.roleid
            });
        }
    }
}