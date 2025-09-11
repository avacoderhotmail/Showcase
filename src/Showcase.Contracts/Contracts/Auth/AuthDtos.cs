namespace Showcase.Contracts.Contracts.Auth
{
    public record RegisterRequestDto(string Email, string Password, string DisplayName);
    public record LoginRequestDto(string Email, string Password);
    public record AuthResponseDto(string Token, string RefreshToken, DateTime RefreshTokenExpires);
    public record RefreshRequestDto(string RefreshToken);
    public record RevokeRequestDto(string RefreshToken);
    public record LoginResponseDto(
    string Token,
    string RefreshToken,
    string UserId,
    string Email,
    string DisplayName
);
    public record RegisterResponseDto(
        string UserId,
        string Email,
        string DisplayName
    );
}
