// DTOs/UserDto.cs
using System.ComponentModel.DataAnnotations;

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
        [Required]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
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
