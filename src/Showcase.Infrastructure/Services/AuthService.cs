using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Showcase.Domain.Entities;
using Showcase.Infrastructure.Data;
using Showcase.Application.Interfaces;
using Showcase.Contracts.Contracts.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly AppDbContext _db;

    public AuthService(UserManager<ApplicationUser> userManager, ITokenService tokenService, AppDbContext db)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _db = db;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto, string ipAddress)
    {
        var user = new ApplicationUser
        {
            Email = dto.Email,
            UserName = dto.Email,
            DisplayName = dto.DisplayName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, "User");

        var refreshToken = _tokenService.GenerateRefreshToken(ipAddress);
        refreshToken.UserId = user.Id;
        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();

        var roles = await _userManager.GetRolesAsync(user);
        var jwt = _tokenService.GenerateJwtToken(user, roles);

        return new AuthResponseDto(jwt, refreshToken.Token, refreshToken.Expires);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, string ipAddress)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            throw new UnauthorizedAccessException("Invalid credentials");

        var refreshToken = _tokenService.GenerateRefreshToken(ipAddress);
        refreshToken.UserId = user.Id;
        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();

        var roles = await _userManager.GetRolesAsync(user);
        var jwt = _tokenService.GenerateJwtToken(user, roles);

        return new AuthResponseDto(jwt, refreshToken.Token, refreshToken.Expires);
    }

    public async Task<AuthResponseDto> RefreshAsync(RefreshRequestDto dto, string ipAddress)
    {
        var existingToken = await _db.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == dto.RefreshToken);

        if (existingToken == null || existingToken.Revoked || existingToken.Expires < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid refresh token");

        existingToken.Revoked = true;
        existingToken.RevokedAt = DateTime.UtcNow;
        existingToken.RevokedByIp = ipAddress;

        var newToken = _tokenService.GenerateRefreshToken(ipAddress);
        newToken.UserId = existingToken.UserId;
        existingToken.ReplacedByToken = newToken.Token;

        _db.RefreshTokens.Add(newToken);
        await _db.SaveChangesAsync();

        var roles = await _userManager.GetRolesAsync(existingToken.User);
        var jwt = _tokenService.GenerateJwtToken(existingToken.User, roles);

        return new AuthResponseDto(jwt, newToken.Token, newToken.Expires);
    }

    public async Task RevokeAsync(RevokeRequestDto dto, string ipAddress)
    {
        var token = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == dto.RefreshToken);
        if (token == null || token.Revoked) return;

        token.Revoked = true;
        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;

        await _db.SaveChangesAsync();
    }
}
