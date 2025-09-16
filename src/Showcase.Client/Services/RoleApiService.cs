using System.Net.Http.Json;

public class RoleApiService : IRoleApiService
{
    private readonly HttpClient _http;

    public RoleApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<string>> GetRolesAsync()
    {
        return await _http.GetFromJsonAsync<IEnumerable<string>>("api/roles")
               ?? Enumerable.Empty<string>();
    }
}
