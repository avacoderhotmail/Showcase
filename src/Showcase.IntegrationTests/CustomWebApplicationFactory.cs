using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Showcase.Api;
using Showcase.Infrastructure.Data;
using System.IO;
using System.Linq;

namespace Showcase.IntegrationTests
{

    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Replace IWebHostEnvironment with a test one
                services.RemoveAll(typeof(IWebHostEnvironment));
                //var envDescriptor = services.SingleOrDefault(
                //    d => d.ServiceType == typeof(IWebHostEnvironment));

                //if (envDescriptor != null)
                //    services.Remove(envDescriptor);

                var tempPath = Path.Combine(Path.GetTempPath(), "ShowcaseTestImages");
                
                // Ensure the directory exists before using it with PhysicalFileProvider
                if (!Directory.Exists(tempPath))
                    Directory.CreateDirectory(tempPath);
                
                services.AddSingleton<IWebHostEnvironment>(sp =>
                {
                    return new TestWebHostEnvironment
                    {
                        WebRootPath = tempPath,
                        ContentRootPath = Directory.GetCurrentDirectory(),
                        WebRootFileProvider = new PhysicalFileProvider(tempPath),
                        ContentRootFileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory()),
                        ApplicationName = "Showcase.Api"
                    };
                });

                // Replace real DB with in-memory
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IntegrationTests");
                });

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
            });
        }
    }
}
