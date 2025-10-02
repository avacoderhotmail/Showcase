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
        private readonly IBlobService _blobService;

        public ProductService(AppDbContext db, IWebHostEnvironment env, IConfiguration config, IBlobService blobService)
        {
            _db = db;
            _env = env;
            _baseUrl = config["AppBaseUrl"] ?? "https://localhost:8000";
            _blobService = blobService;
        }

        public async Task<ProductReadDto> CreateAsync(ProductCreateDto dto, IFormFile? imageFile = null)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price
            };

            if (imageFile != null && imageFile.Length > 0)
            {
                // Generate a unique blob name
                var blobName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";

                // Upload to blob storage
                using var stream = imageFile.OpenReadStream();
                await _blobService.UploadFileAsync(stream, blobName);

                // Store blob name in DB
                product.ImageFileName = blobName;
            }

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            // Return DTO with temporary SAS URL for client
            return await MapToReadDtoWithSasAsync(product);
        }
        public async Task<IEnumerable<ProductReadDto>> GetAllAsync()
        {
            var products = await _db.Products.ToListAsync();

            var result = new List<ProductReadDto>();
            foreach (var product in products)
            {
                result.Add(await MapToReadDtoWithSasAsync(product));
            }

            return result;
        }

        public async Task<ProductReadDto?> GetByIdAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);
            return product == null ? null : await MapToReadDtoWithSasAsync(product);
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
                // Delete old image from Blob Storage if exists
                if (!string.IsNullOrEmpty(product.ImageFileName))
                {
                    var oldFileName = Path.GetFileName(new Uri(product.ImageFileName).AbsolutePath);
                    await _blobService.DeleteFileAsync(oldFileName);
                }

                // Upload new image to Blob Storage
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                using var stream = imageFile.OpenReadStream();
                var fileUrl = await _blobService.UploadFileAsync(stream, fileName);

                product.ImageFileName = fileUrl; // store the URL directly
            }

            await _db.SaveChangesAsync();
            return await MapToReadDtoWithSasAsync(product);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return false;

            if (!string.IsNullOrEmpty(product.ImageFileName))
            {
                var oldFileName = Path.GetFileName(new Uri(product.ImageFileName).AbsolutePath);
                await _blobService.DeleteFileAsync(oldFileName);
            }

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return true;
        }

        //private static ProductReadDto MapToReadDto(Product product, string baseUrl)
        //{
        //    var imageUrl = string.IsNullOrEmpty(product.ImageFileName)
        //        ? null
        //        : product.ImageFileName; // full blob URL from Azure

        //    return new ProductReadDto
        //    {
        //        Id = product.Id,
        //        Name = product.Name,
        //        Description = product.Description,
        //        Price = product.Price,
        //        CreatedAt = product.CreatedAt,
        //        ImageUrl = imageUrl
        //    };
        //}

        // Helper to map to DTO with SAS URL
        private async Task<ProductReadDto> MapToReadDtoWithSasAsync(Product product)
        {
            string? imageUrl = null;
            if (!string.IsNullOrEmpty(product.ImageFileName))
            {
                // Generate a temporary SAS URI for private blob access
                imageUrl = await _blobService.GetSasUri(product.ImageFileName);
            }

            ProductReadDto dto = new ProductReadDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CreatedAt = product.CreatedAt,
                ImageUrl = imageUrl
            };

            return dto;
        }
    }

}
