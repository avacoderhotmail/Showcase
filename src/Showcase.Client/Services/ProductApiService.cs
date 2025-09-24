using Microsoft.AspNetCore.Components.Forms;
using Showcase.Contracts.Contracts.Product;
using System.Net.Http.Json;

namespace Showcase.Client.Services;

public class ProductApiService : IProductApiService
{
    private readonly HttpClient _http;

    public ProductApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsAsync()
    {
        return await _http.GetFromJsonAsync<IEnumerable<ProductDto>>("api/products")
               ?? Enumerable.Empty<ProductDto>();
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<ProductDto>($"api/products/{id}");
    }

    public async Task<ProductDto?> CreateProductAsync(ProductCreateDto dto, IBrowserFile? imageFile)
    {
        var content = new MultipartFormDataContent
    {
        { new StringContent(dto.Name ?? ""), nameof(dto.Name) },
        { new StringContent(dto.Description ?? ""), nameof(dto.Description) },
        { new StringContent(dto.Price.ToString(System.Globalization.CultureInfo.InvariantCulture)), nameof(dto.Price) }
    };

        if (imageFile != null)
        {
            var stream = imageFile.OpenReadStream(5 * 1024 * 1024); // don't dispose early
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imageFile.ContentType);
            content.Add(fileContent, "imageFile", imageFile.Name);
        }

        var response = await _http.PostAsync("api/products", content);

        var body = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response: {response.StatusCode} - {body}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ProductDto>();
    }

    public async Task<ProductDto?> UpdateProductAsync(int id, ProductUpdateDto dto, IBrowserFile? imageFile)
    {
        var content = new MultipartFormDataContent();
        AddProductFields(content, dto.Name, dto.Description, dto.Price);

        // Add image file if provided
        if (imageFile != null)
        {
            var stream = imageFile.OpenReadStream(5 * 1024 * 1024); // Limit to 5 MB
            content.Add(new StreamContent(stream), "imageFile", imageFile.Name);
        }

        var response = await _http.PutAsync($"api/products/{id}", content);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ProductDto>();
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/products/{id}");
        return response.IsSuccessStatusCode;
    }

    private void AddProductFields(MultipartFormDataContent content, string? name, string? desc, decimal price)
    {
        content.Add(new StringContent(name ?? ""), "Name");
        content.Add(new StringContent(desc ?? ""), "Description");
        content.Add(new StringContent(price.ToString(System.Globalization.CultureInfo.InvariantCulture)), "Price");
    }
}
