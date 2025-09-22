using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Showcase.Contracts.Contracts.Product;
using System.Threading.Tasks;


using Xunit;

namespace Showcase.IntegrationTests
{
    public class ProductApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ProductApiTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PostProduct_WithImage_CreatesAndReturnsProduct()
        {
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Test");

            var dto = new ProductCreateDto
            {
                Name = "Integration Product",
                Description = "Desc",
                Price = 19.99m
            };

            var content = new MultipartFormDataContent();
            content.Add(new StringContent(dto.Name), "Name");
            content.Add(new StringContent(dto.Description), "Description");
            content.Add(new StringContent(dto.Price.ToString()), "Price");

            // Simulate a small image file for testing
            var bytes = new byte[] { 255, 216, 255, 224 }; // JPEG header stub
            var imageContent = new ByteArrayContent(bytes);
            imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            content.Add(imageContent, "imageFile", "test.jpg");

            var response = await _client.PostAsync("/api/products", content);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<ProductReadDto>();
            Assert.NotNull(created);
            Assert.Equal("Integration Product", created.Name);
            Assert.NotNull(created.ImageUrl); // Should be set!
        }


        [Fact]
        public async Task GetProduct_ReturnsSavedProduct()
        {
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Test");

            var dto = new ProductCreateDto
            {
                Name = "Get Me",
                Description = "Desc",
                Price = 19.99m
            };

            var content = new MultipartFormDataContent();
            content.Add(new StringContent(dto.Name), "Name");
            content.Add(new StringContent(dto.Description), "Description");
            content.Add(new StringContent(dto.Price.ToString()), "Price");

            // Simulate a small image file for testing
            var bytes = new byte[] { 255, 216, 255, 224 }; // JPEG header stub
            var imageContent = new ByteArrayContent(bytes);
            imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            content.Add(imageContent, "imageFile", "test.jpg");

            var response = await _client.PostAsync("/api/products", content);
            var created = await response.Content.ReadFromJsonAsync<ProductReadDto>();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var getResponse = await _client.GetAsync($"/api/products/{created.Id}");
            getResponse.EnsureSuccessStatusCode();

            var fetched = await getResponse.Content.ReadFromJsonAsync<ProductReadDto>();
            Assert.Equal(created.Id, fetched.Id);
            Assert.Equal("Get Me", fetched.Name);
        }

        [Fact]
        public async Task DeleteProduct_RemovesFromDatabase()
        {
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Test");

            var dto = new ProductCreateDto
            {
                Name = "Integration Product",
                Description = "Desc",
                Price = 19.99m
            };

            var content = new MultipartFormDataContent();
            content.Add(new StringContent(dto.Name), "Name");
            content.Add(new StringContent(dto.Description), "Description");
            content.Add(new StringContent(dto.Price.ToString()), "Price");

            // Simulate a small image file for testing
            var bytes = new byte[] { 255, 216, 255, 224 }; // JPEG header stub
            var imageContent = new ByteArrayContent(bytes);
            imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            content.Add(imageContent, "imageFile", "test.jpg");

            var response = await _client.PostAsync("/api/products", content);
            var created = await response.Content.ReadFromJsonAsync<ProductReadDto>();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var deleteResponse = await _client.DeleteAsync($"/api/products/{created.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/products/{created.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}