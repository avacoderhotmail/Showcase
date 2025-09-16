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
    }

    public record ProductReadDto(int Id, string Name, string Description, decimal Price, DateTime CreatedAt);

    public class ProductDto()
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }

}
