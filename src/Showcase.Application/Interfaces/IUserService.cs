// Interfaces/IUserService.cs
using Showcase.Application.DTOs;

namespace Showcase.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(string userId);
        Task<UserDto> CreateUserAsync(CreateUserDto dto);
        Task<UserDto?> UpdateUserAsync(string userId, UpdateUserDto dto);
        Task<bool> DeleteUserAsync(string userId);
    }
}
