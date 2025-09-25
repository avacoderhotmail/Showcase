namespace Showcase.Client.Services
{
    public interface IBreadcrumbService
    {
        Task<string?> ResolveSegmentAsync(string segment, string parentPath);
    }
}
