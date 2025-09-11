using Showcase.Contracts.Contracts.User;
using System.Net.Http.Json;

namespace Showcase.Client.Services;

public class UserApiService : IUserApiService
{
    private readonly HttpClient _http;

    public UserApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        return await _http.GetFromJsonAsync<IEnumerable<UserDto>>("api/users")
               ?? Enumerable.Empty<UserDto>();
    }

    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        return await _http.GetFromJsonAsync<UserDto>($"api/users/{id}");
    }

    public async Task<UserDto?> CreateUserAsync(CreateUserDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/users", dto);
        return await response.Content.ReadFromJsonAsync<UserDto>();
    }

    public async Task<UserDto?> UpdateUserAsync(string id, UpdateUserDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/users/{id}", dto);
        return await response.Content.ReadFromJsonAsync<UserDto>();
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        var response = await _http.DeleteAsync($"api/users/{id}");
        return response.IsSuccessStatusCode;
    }
}
