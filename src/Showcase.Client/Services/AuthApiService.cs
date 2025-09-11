using Showcase.Contracts.Contracts.Auth;
using System.Net.Http.Json;

namespace Showcase.Client.Services;


public class AuthApiService : IAuthApiService
{
    private readonly HttpClient _http;

    public AuthApiService(HttpClient http) => _http = http;

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto) =>
        await _http.PostAsJsonAsync("api/auth/login", dto)
                   .Result.Content.ReadFromJsonAsync<LoginResponseDto>();

    public async Task<RegisterResponseDto?> RegisterAsync(RegisterRequestDto dto) =>
        await _http.PostAsJsonAsync("api/auth/register", dto)
                   .Result.Content.ReadFromJsonAsync<RegisterResponseDto>();

    public async Task<LoginResponseDto?> RefreshTokenAsync(RefreshRequestDto dto) =>
        await _http.PostAsJsonAsync("api/auth/refresh", dto)
                   .Result.Content.ReadFromJsonAsync<LoginResponseDto>();
}
