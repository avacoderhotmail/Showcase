using Microsoft.EntityFrameworkCore;
using Showcase.Contracts.Contracts.Product;
using Showcase.Infrastructure.Data;
using Showcase.Application.Interfaces;

namespace Showcase.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;

        public ProductService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ProductReadDto> CreateAsync(ProductCreateDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price
            };
            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return MapToReadDto(product);
        }

        public async Task<IEnumerable<ProductReadDto>> GetAllAsync()
        {
            return await _db.Products
                .Select(p => MapToReadDto(p))
                .ToListAsync();
        }

        public async Task<ProductReadDto?> GetByIdAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);
            return product == null ? null : MapToReadDto(product);
        }

        public async Task<ProductReadDto?> UpdateAsync(int id, ProductUpdateDto dto)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return null;

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;

            await _db.SaveChangesAsync();
            return MapToReadDto(product);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return false;

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return true;
        }

        private static ProductReadDto MapToReadDto(Product product)
            => new ProductReadDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CreatedAt = product.CreatedAt
            };
    }

}
