namespace Showcase.Contracts.Contracts.Product
{
    public record ProductCreateDto(string Name, string Description, decimal Price);
    public record ProductUpdateDto(string Name, string Description, decimal Price);
    public record ProductReadDto(int Id, string Name, string Description, decimal Price, DateTime CreatedAt);

    public record ProductDto ( int Id, string Name, string Description, decimal Price);

}
