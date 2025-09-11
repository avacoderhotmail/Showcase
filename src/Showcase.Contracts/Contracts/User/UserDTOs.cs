// DTOs/UserDto.cs
namespace Showcase.Contracts.Contracts.User
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public IEnumerable<string> Roles { get; set; } = Array.Empty<string>();
    }
}

// DTOs/CreateUserDto.cs
namespace Showcase.Contracts.Contracts.User
{
    public class CreateUserDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public IEnumerable<string> Roles { get; set; } = Array.Empty<string>();
    }
}

// DTOs/UpdateUserDto.cs
namespace Showcase.Contracts.Contracts.User
{
    public class UpdateUserDto
    {
        public string DisplayName { get; set; } = string.Empty;
        public IEnumerable<string> Roles { get; set; } = Array.Empty<string>();
    }
}
