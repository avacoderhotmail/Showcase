using Microsoft.AspNetCore.Components.Forms;
using Showcase.Contracts.Contracts.Product;

namespace Showcase.Client.Services;

public interface IProductApiService
{
    Task<IEnumerable<ProductDto>> GetProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<ProductDto?> CreateProductAsync(ProductCreateDto dto, IBrowserFile? imageFile = null);
    Task<ProductDto?> UpdateProductAsync(int id, ProductUpdateDto dto, IBrowserFile? imageFile = null);
    Task<bool> DeleteProductAsync(int id);
}
