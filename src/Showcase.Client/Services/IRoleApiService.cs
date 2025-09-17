public interface IRoleApiService
{
    Task<IEnumerable<string>> GetRolesAsync();
    Task<bool> CreateRoleAsync(string roleName);
}
