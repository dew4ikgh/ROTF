using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ROTF.Server.Services.Interfaces;
using ROTF.Server.DTOs;
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

        /// <summary>
        /// Реєструє нового гравця в системі.
        /// </summary>
        /// <remarks>
        /// Приклад запиту:
        ///
        ///     POST /api/v1/auth/register
        ///     {
        ///        "username": "Nazarii",
        ///        "email": "nazarii@polytech.cv.ua",
        ///        "password": "SecurePass123"
        ///     }
        ///
        /// Приклад успішної відповіді (200 OK):
        ///
        ///     { "message": "Реєстрація успішна" }
        ///
        /// Приклад помилки (409 Conflict) — логін або email вже зайняті:
        ///
        ///     { "message": "Користувач з таким логіном вже існує" }
        /// </remarks>
        /// <param name="dto">Логін, email та пароль нового користувача</param>
        /// <response code="200">Гравця успішно зареєстровано</response>
        /// <response code="400">Помилка валідації (наприклад, короткий пароль)</response>
        /// <response code="409">Логін або email вже зайняті</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
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

        /// <summary>
        /// Автентифікує гравця та видає JWT-токен для подальших запитів.
        /// </summary>
        /// <remarks>
        /// Приклад запиту:
        ///
        ///     POST /api/v1/auth/login
        ///     {
        ///        "username": "Nazarii",
        ///        "password": "SecurePass123"
        ///     }
        ///
        /// Приклад успішної відповіді (200 OK):
        ///
        ///     {
        ///        "message": "Вхід успішний",
        ///        "token": "eyJhbGciOiJIUzI1NiIs...",
        ///        "id": 2,
        ///        "username": "Nazarii",
        ///        "role": 1
        ///     }
        /// </remarks>
        /// <param name="dto">Логін і пароль користувача</param>
        /// <response code="200">Вхід успішний, у відповіді — JWT-токен</response>
        /// <response code="400">Помилка валідації вхідних даних</response>
        /// <response code="401">Невірний логін або пароль</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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