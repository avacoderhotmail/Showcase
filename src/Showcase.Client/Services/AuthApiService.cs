using Microsoft.JSInterop;
using Showcase.Contracts.Contracts.Auth;
using System.Net.Http.Json;

namespace Showcase.Client.Services;

public class AuthApiService : IAuthApiService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;
    private string? _inMemoryAccessToken;
    private string? _inMemoryRefreshToken;
    private const string AccessTokenKey = "showcase_auth_token";
    private const string RefreshTokenKey = "showcase_refresh_token";


    public AuthApiService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", dto);
        if (!response.IsSuccessStatusCode) return null;

        var payload = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        if (payload == null || string.IsNullOrWhiteSpace(payload.Token) || string.IsNullOrWhiteSpace(payload.RefreshToken))
            return null;

        await SetTokensAsync(payload.Token, payload.RefreshToken);

        return payload;
    }

    public async Task<RegisterResponseDto?> RegisterAsync(RegisterRequestDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", dto);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<RegisterResponseDto>();
    }

    public async Task LogoutAsync()
    {
        _inMemoryAccessToken = null;
        _inMemoryRefreshToken = null;

        await _js.InvokeVoidAsync("sessionStorage.removeItem", AccessTokenKey);
        await _js.InvokeVoidAsync("sessionStorage.removeItem", RefreshTokenKey);
    }

    public async Task<string?> GetTokenAsync()
    {
        // prefer in-memory value for performance; fallback to localStorage
        if (!string.IsNullOrWhiteSpace(_inMemoryAccessToken) && IsValid(_inMemoryAccessToken))
            return IsValid(_inMemoryAccessToken) ? _inMemoryAccessToken : null;

        try
        {
            _inMemoryAccessToken = await _js.InvokeAsync<string?>("sessionStorage.getItem", AccessTokenKey);
            _inMemoryRefreshToken ??= await _js.InvokeAsync<string?>("sessionStorage.getItem", RefreshTokenKey);

            if (string.IsNullOrWhiteSpace(_inMemoryAccessToken) || !IsValid(_inMemoryAccessToken))
                _inMemoryAccessToken = null;
        }
        catch
        {
            _inMemoryAccessToken = null;
        }

        return _inMemoryAccessToken;
    }

    private async Task SetTokensAsync(string accessToken, string refreshToken)
    {
        _inMemoryAccessToken = accessToken;
        _inMemoryRefreshToken = refreshToken;

        await _js.InvokeVoidAsync("sessionStorage.setItem", AccessTokenKey, accessToken);
        await _js.InvokeVoidAsync("sessionStorage.setItem", RefreshTokenKey, refreshToken);
    }

    public async Task SetTokenAsync(string? token)
    {
        _inMemoryAccessToken = token;
        if (string.IsNullOrEmpty(token))
            await _js.InvokeVoidAsync("sessionStorage.removeItem", AccessTokenKey);
        else
            await _js.InvokeVoidAsync("sessionStorage.setItem", AccessTokenKey, token);
    }

    private bool IsValid(string token)
    {
        var claims = JwtParser.ParseClaimsFromJwt(token);
        var exp = claims.FirstOrDefault(c => c.Type == "exp")?.Value;
        if (exp == null) return false;

        var expDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp));
        return expDate > DateTimeOffset.UtcNow;
    }

    public async Task<bool> RefreshTokenAsync()
    {
        if (string.IsNullOrWhiteSpace(_inMemoryRefreshToken))
            _inMemoryRefreshToken = await _js.InvokeAsync<string?>("sessionStorage.getItem", RefreshTokenKey);

        if (string.IsNullOrWhiteSpace(_inMemoryRefreshToken))
            return false;

        var response = await _http.PostAsJsonAsync("api/auth/refresh", new RefreshRequestDto( _inMemoryRefreshToken ));
        if (!response.IsSuccessStatusCode) return false;

        var payload = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        if(payload == null || string.IsNullOrWhiteSpace(payload.Token) || string.IsNullOrWhiteSpace(payload.RefreshToken))
            return false;

        await SetTokensAsync(payload.Token, payload.RefreshToken);

        return true;
    }

    public static bool IsExpiringSoon(string? token, int bufferSeconds = 120)
    {
        if (string.IsNullOrWhiteSpace(token)) return true;

        var claims = JwtParser.ParseClaimsFromJwt(token);
        var exp = claims.FirstOrDefault(c => c.Type == "exp")?.Value;
        if (exp == null) return true;

        var expDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp));
        return expDate <= DateTimeOffset.UtcNow.AddSeconds(bufferSeconds);
    }

}
