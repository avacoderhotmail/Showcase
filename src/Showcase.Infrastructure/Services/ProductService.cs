using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Showcase.Application.Interfaces;
using Showcase.Contracts.Contracts.Product;
using Showcase.Infrastructure.Data;
using Showcase.Domain.Entities;

namespace Showcase.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly string _baseUrl;

        public ProductService(AppDbContext db, IWebHostEnvironment env, IConfiguration config)
        {
            _db = db;
            _env = env;
            _baseUrl = config["AppBaseUrl"] ?? "https://localhost:8000";
        }

        public async Task<ProductReadDto> CreateAsync(ProductCreateDto dto, IFormFile? imageFile = null)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price
            };

            // Handle image upload
            if (imageFile != null)
            {
                var imagesFolder = Path.Combine(_env.ContentRootPath, "uploads");
                Directory.CreateDirectory(imagesFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                var filePath = Path.Combine(imagesFolder, fileName);

                using var stream = System.IO.File.Create(filePath);
                await imageFile.CopyToAsync(stream);

                product.ImageFileName = fileName;
            }

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return MapToReadDto(product, _baseUrl);
        }

        public async Task<IEnumerable<ProductReadDto>> GetAllAsync()
        {
            return await _db.Products
                .Select(p => MapToReadDto(p, _baseUrl))
                .ToListAsync();
        }

        public async Task<ProductReadDto?> GetByIdAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);
            return product == null ? null : MapToReadDto(product, _baseUrl);
        }

        public async Task<ProductReadDto?> UpdateAsync(int id, ProductUpdateDto dto, IFormFile? imageFile = null)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return null;

            // Update fields
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;

            // Handle image update
            if (imageFile != null && imageFile.Length > 0)
            {
                var imagesFolder = Path.Combine(_env.ContentRootPath, "uploads");
                Directory.CreateDirectory(imagesFolder);

                // Delete old image if exists
                if (!string.IsNullOrEmpty(product.ImageFileName))
                {
                    var oldImagePath = Path.Combine(imagesFolder, product.ImageFileName);
                    if (File.Exists(oldImagePath))
                        File.Delete(oldImagePath);
                }

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                var filePath = Path.Combine(imagesFolder, fileName);

                using var stream = System.IO.File.Create(filePath);
                await imageFile.CopyToAsync(stream);

                product.ImageFileName = fileName;
            }

            await _db.SaveChangesAsync();
            return MapToReadDto(product, _baseUrl);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return false;

            // Delete image from filesystem
            if (!string.IsNullOrEmpty(product.ImageFileName))
            {
                var imagesFolder = Path.Combine(_env.ContentRootPath, "uploads");
                var imagePath = Path.Combine(imagesFolder, product.ImageFileName);
                if (File.Exists(imagePath))
                    File.Delete(imagePath);
            }

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return true;
        }

        private static ProductReadDto MapToReadDto(Product product, string baseUrl)
        {
            var imageUrl = string.IsNullOrEmpty(product.ImageFileName) ? null
                : $"{baseUrl}/uploads/{product.ImageFileName}"; // relative URL for Blazor client

            return new ProductReadDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CreatedAt = product.CreatedAt,
                ImageUrl = imageUrl
            };
        }
    }

}
