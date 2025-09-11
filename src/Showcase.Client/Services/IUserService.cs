using Showcase.Contracts.Contracts.User;

namespace Showcase.Client.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetUsersAsync();
    Task<UserDto?> GetUserByIdAsync(string id);
    Task<UserDto?> CreateUserAsync(CreateUserDto dto);
    Task<UserDto?> UpdateUserAsync(string id, UpdateUserDto dto);
    Task<bool> DeleteUserAsync(string id);
}
