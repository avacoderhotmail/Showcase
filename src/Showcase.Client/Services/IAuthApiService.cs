using Showcase.Contracts.Contracts.Auth;

namespace Showcase.Client.Services
{
    public interface IAuthApiService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto);
        Task<RegisterResponseDto?> RegisterAsync(RegisterRequestDto dto);
        Task<LoginResponseDto?> RefreshTokenAsync(RefreshRequestDto dto);
    }
}
