public interface IRoleApiService
{
    Task<IEnumerable<string>> GetRolesAsync();
}
