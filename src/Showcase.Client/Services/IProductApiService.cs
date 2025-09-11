using Showcase.Contracts.Contracts.Product;

namespace Showcase.Client.Services;

public interface IProductApiService
{
    Task<IEnumerable<ProductDto>> GetProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(Guid id);
    Task<ProductDto?> CreateProductAsync(ProductCreateDto dto);
    Task<ProductDto?> UpdateProductAsync(Guid id, ProductUpdateDto dto);
    Task<bool> DeleteProductAsync(Guid id);
}
