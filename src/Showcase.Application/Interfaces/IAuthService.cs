namespace Showcase.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto, string ipAddress);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, string ipAddress);
        Task<AuthResponseDto> RefreshAsync(RefreshRequestDto dto, string ipAddress);
        Task RevokeAsync(RevokeRequestDto dto, string ipAddress);
    }
}