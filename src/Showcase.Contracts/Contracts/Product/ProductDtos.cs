namespace Showcase.Contracts.Contracts.Product
{
    public class ProductCreateDto() {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0.0m;
    }

    public record ProductUpdateDto(string Name, string Description, decimal Price);
    public record ProductReadDto(int Id, string Name, string Description, decimal Price, DateTime CreatedAt);

    public record ProductDto ( int Id, string Name, string Description, decimal Price);

}
