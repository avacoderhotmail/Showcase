public record RegisterRequestDto(string Email, string Password, string DisplayName);
public record LoginRequestDto(string Email, string Password);
public record AuthResponseDto(string JwtToken, string RefreshToken, DateTime RefreshTokenExpires);
public record RefreshRequestDto(string RefreshToken);
public record RevokeRequestDto(string RefreshToken);
