using Showcase.Contracts.Contracts.Auth;

namespace Showcase.Client.Services
{
    public interface IAuthApiService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto);
        Task LogoutAsync();
        Task<string?> GetTokenAsync();         // required by the message handler
        Task SetTokenAsync(string? token);     // optional, for manual token management
        Task<bool> RefreshTokenAsync();  // refresh the token using the refresh token
    }
}
