using Showcase.Contracts.Contracts.Product;
using System.Net.Http.Json;

namespace Showcase.Client.Services;

public class ProductService : IProductService
{
    private readonly HttpClient _http;

    public ProductService(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsAsync()
    {
        return await _http.GetFromJsonAsync<IEnumerable<ProductDto>>("api/products")
               ?? Enumerable.Empty<ProductDto>();
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<ProductDto>($"api/products/{id}");
    }

    public async Task<ProductDto?> CreateProductAsync(ProductCreateDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/products", dto);
        return await response.Content.ReadFromJsonAsync<ProductDto>();
    }

    public async Task<ProductDto?> UpdateProductAsync(Guid id, ProductUpdateDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/products/{id}", dto);
        return await response.Content.ReadFromJsonAsync<ProductDto>();
    }

    public async Task<bool> DeleteProductAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"api/products/{id}");
        return response.IsSuccessStatusCode;
    }
}
