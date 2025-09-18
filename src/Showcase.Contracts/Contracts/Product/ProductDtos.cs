namespace Showcase.Contracts.Contracts.Product
{
    public class ProductCreateDto()
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0.0m;
    }

    public class ProductUpdateDto()
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        string? ImageUrl { get; set; } // full URL to the image
    }

    public class ProductReadDto()
    {
        public int Id { get; init; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ImageUrl { get; set; } // full URL to the image

    }

    public class ProductDto()
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; } // full URL to the image
    }

}
