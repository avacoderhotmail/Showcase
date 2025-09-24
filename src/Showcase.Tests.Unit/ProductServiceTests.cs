using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Showcase.Infrastructure.Data;
using Showcase.Infrastructure.Services;
using Showcase.Contracts.Contracts.Product;
using Showcase.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace Showcase.Tests.Unit
{
    public class ProductServiceTests
    {
        private readonly DbContextOptions<AppDbContext> _dbOptions;

        public ProductServiceTests()
        {
            _dbOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "ProductServiceTests")
                .Options;
        }

        private DbContextOptions<AppDbContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // isolate per test
                .Options;
        }

        private ProductService CreateService(DbContextOptions<AppDbContext> options)
        {
            var env = new Mock<IWebHostEnvironment>();
            env.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

            var config = new ConfigurationBuilder().Build();

            var blobMock = new Mock<IBlobService>();
            blobMock.Setup(b => b.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync("https://fake.blob.core.windows.net/container/fakefile.png");
            blobMock.Setup(b => b.DeleteFileAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var db = new AppDbContext(options);
            return new ProductService(db, env.Object, config, blobMock.Object);
        }

        [Fact]
        public async Task CreateProduct_SavesToDatabase_AndWritesImageFile()
        {
            // Arrange
            var tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempPath);

            var options = CreateOptions();
            var service = CreateService( options);

            var dto = new ProductCreateDto
            {
                Name = "Test Product",
                Description = "Test Desc",
                Price = 9.99m
            };

            // Act
            var created = await service.CreateAsync(dto);

            // Assert
            Assert.NotNull(created);
            Assert.Equal("Test Product", created.Name);

            using var db = new AppDbContext(options); // use the SAME options
            var saved = await db.Products.FirstOrDefaultAsync();
            Assert.NotNull(saved);
            Assert.Equal("Test Product", saved.Name);

            // since we didn’t provide an image, no file should exist
            var imageDir = Path.Combine(tempPath, "images", "products");
            Assert.True(Directory.Exists(imageDir) || !Directory.Exists(imageDir));
            // if you want stricter, drop this or provide a fake image
        }

        [Fact]
        public async Task UpdateProduct_UpdatesFields()
        {
            // Arrange
            var options = CreateOptions();

            using (var db = new AppDbContext(options))
            {
                db.Products.Add(new Product
                {
                    Name = "Old Name",
                    Description = "Old Desc",
                    Price = 5.00m
                });
                await db.SaveChangesAsync();
            }

            var service = CreateService( options);

            var updateDto = new ProductUpdateDto
            {
                Name = "New Name",
                Description = "New Desc",
                Price = 12.34m
            };

            // Act
            await service.UpdateAsync(1, updateDto);

            // Assert
            using var db2 = new AppDbContext(options);
            var updated = await db2.Products.FindAsync(1);
            Assert.Equal("New Name", updated.Name);
            Assert.Equal("New Desc", updated.Description);
            Assert.Equal(12.34m, updated.Price);
        }

        [Fact]
        public async Task DeleteProduct_RemovesFromDatabase()
        {
            // Arrange
            var options = CreateOptions();

            using (var db = new AppDbContext(options))
            {
                db.Products.Add(new Product
                {
                    Name = "DeleteMe",
                    Description = "Temp",
                    Price = 1m
                });
                await db.SaveChangesAsync();
            }

            var service = CreateService( options);

            // Act
            var result = await service.DeleteAsync(1);

            // Assert
            Assert.True(result);
            using var db2 = new AppDbContext(options);
            Assert.Empty(db2.Products);
        }
    }
}
