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
using Showcase.Application.Interfaces;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;

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

                // Remove any existing IBlobService registration
                var blobDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IBlobService));
                if (blobDescriptor != null)
                    services.Remove(blobDescriptor);

                // Add a mock IBlobService
                var blobMock = new Mock<IBlobService>();
                blobMock.Setup(b => b.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                        .ReturnsAsync("https://fake.blob.core.windows.net/uploads/fake.jpg");
                blobMock.Setup(b => b.DeleteFileAsync(It.IsAny<string>()))
                        .Returns(Task.CompletedTask);

                services.AddScoped(_ => blobMock.Object);

            });
        }
    }
}
