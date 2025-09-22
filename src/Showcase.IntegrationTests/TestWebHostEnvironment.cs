using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

public class TestWebHostEnvironment : IWebHostEnvironment
{
    public string WebRootPath { get; set; }
    // Implement other props/methods as needed, can be stubs for most tests:
    public string EnvironmentName { get; set; } = "Development";
    public string ApplicationName { get; set; }
    public string ContentRootPath { get; set; }
    public IFileProvider WebRootFileProvider { get; set; }
    public IFileProvider ContentRootFileProvider { get; set; }
}
