using System.Net.Http.Json;
using Showcase.Contracts.Contracts.User; // or duplicate DTOs in Client if you don't want coupling

namespace Showcase.Client.Services
{
    public class UserApiService
    {
        private readonly HttpClient _http;

        public UserApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<UserDto>?> GetUsersAsync()
            => await _http.GetFromJsonAsync<List<UserDto>>("api/users");

        public async Task<UserDto?> GetUserAsync(string id)
            => await _http.GetFromJsonAsync<UserDto>($"api/users/{id}");

        public async Task<UserDto?> CreateUserAsync(CreateUserDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/users", dto);
            return await response.Content.ReadFromJsonAsync<UserDto>();
        }
    }
}
