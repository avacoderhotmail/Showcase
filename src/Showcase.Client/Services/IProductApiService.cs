using Showcase.Contracts.Contracts.Product;

namespace Showcase.Client.Services;

public interface IProductApiService
{
    Task<IEnumerable<ProductDto>> GetProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<ProductDto?> CreateProductAsync(ProductCreateDto dto);
    Task<ProductDto?> UpdateProductAsync(int id, ProductUpdateDto dto);
    Task<bool> DeleteProductAsync(int id);
}
