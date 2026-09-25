using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ROTF.Api.Dtos;
using ROTF.Server.Models;
using ROTF.Server.Services.Interfaces;
using ROTF.Server.Common;
using ROTF.Server.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ROTF.Server.Services
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<User> _userRepo; // Впроваджуємо репозиторій замість контексту
        private readonly IConfiguration _configuration;

        public AuthService(IGenericRepository<User> userRepo, IConfiguration configuration)
        {
            _userRepo = userRepo;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<bool>> RegisterAsync(RegisterDto dto)
        {
            // Шукаємо користувачів із таким іменем
            var existingUsers = await _userRepo.FindAsync(u => u.username == dto.Username);
            if (existingUsers.Any())
                return ServiceResponse<bool>.Fail(ServiceResultType.Conflict, "Користувач з таким логіном вже існує");

            var newUser = new User
            {
                username = dto.Username,
                email = dto.Email,
                passwordhash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                roleid = 1
            };

            await _userRepo.AddAsync(newUser);
            await _userRepo.SaveAsync(); // Схороняємо зміни через репозиторій

            return ServiceResponse<bool>.Ok(true, "Реєстрація успішна");
        }

        public async Task<ServiceResponse<AuthResponse>> LoginAsync(LoginDto dto)
        {
            // Отримуємо користувача за логіном
            var users = await _userRepo.FindAsync(u => u.username == dto.Username);
            var user = users.FirstOrDefault();

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.passwordhash))
                return ServiceResponse<AuthResponse>.Fail(ServiceResultType.Unauthorized, "Невірний логін або пароль");

            var response = new AuthResponse
            {
                Token = GenerateJwtToken(user),
                User = user
            };

            return ServiceResponse<AuthResponse>.Ok(response, "Вхід успішний");
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.username),
                new Claim("id", user.id.ToString()),
                new Claim(ClaimTypes.Role, user.roleid.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}