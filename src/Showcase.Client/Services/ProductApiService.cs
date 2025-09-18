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
        var content = new MultipartFormDataContent();

        // add dto properties
        content.Add(new StringContent(dto.Name ?? ""), "Name");
        content.Add(new StringContent(dto.Description ?? ""), "Description");
        content.Add(new StringContent(dto.Price.ToString(System.Globalization.CultureInfo.InvariantCulture)), "Price");

        // Add image file if provided
        if (imageFile != null)
        {
            var stream = imageFile.OpenReadStream(5 * 1024 * 1024); // Limit to 5 MB
            content.Add(new StreamContent(stream), "imageFile", imageFile.Name);
        }

        var response = await _http.PostAsJsonAsync("api/products", content);
        if(!response.IsSuccessStatusCode)
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
        if(!response.IsSuccessStatusCode)
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
