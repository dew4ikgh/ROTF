using ROTF.Api.Dtos;
using ROTF.Server.Models;
using ROTF.Server.Common;

namespace ROTF.Server.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<bool>> RegisterAsync(RegisterDto dto);
        Task<ServiceResponse<AuthResponse>> LoginAsync(LoginDto dto);
    }

    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public User User { get; set; } = null!;
    }
}