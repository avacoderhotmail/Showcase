using Microsoft.JSInterop;
using Showcase.Contracts.Contracts.Auth;
using System.Net.Http.Json;

namespace Showcase.Client.Services;

public class AuthApiService : IAuthApiService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;
    private string? _inMemoryToken;
    private const string TokenKey = "showcase_auth_token";

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
        var token = payload?.Token;
        _inMemoryToken = token;

        if (!string.IsNullOrEmpty(token))
            await _js.InvokeVoidAsync("sessionStorage.setItem", TokenKey, token);

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
        _inMemoryToken = null;
        await _js.InvokeVoidAsync("sessionStorage.removeItem", TokenKey);
    }

    public async Task<string?> GetTokenAsync()
    {
        // prefer in-memory value for performance; fallback to localStorage
        if (!string.IsNullOrWhiteSpace(_inMemoryToken))
            return IsValid(_inMemoryToken) ? _inMemoryToken : null;

        try
        {
            _inMemoryToken = await _js.InvokeAsync<string?>("sessionStorage.getItem", TokenKey);
            if(string.IsNullOrWhiteSpace(_inMemoryToken) || !IsValid(_inMemoryToken))
                _inMemoryToken = null;
        }
        catch
        {
            _inMemoryToken = null;
        }

        return _inMemoryToken;
    }

    public async Task SetTokenAsync(string? token)
    {
        _inMemoryToken = token;
        if (string.IsNullOrEmpty(token))
            await _js.InvokeVoidAsync("sessionStorage.removeItem", TokenKey);
        else
            await _js.InvokeVoidAsync("sessionStorage.setItem", TokenKey, token);
    }

    private bool IsValid(string token)
    {
        var claims = JwtParser.ParseClaimsFromJwt(token);
        var exp = claims.FirstOrDefault(c => c.Type == "exp")?.Value;
        if (exp == null) return false;

        var expDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp));
        return expDate > DateTimeOffset.UtcNow;
    }
}
