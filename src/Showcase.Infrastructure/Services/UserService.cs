using Microsoft.AspNetCore.Identity;
using Showcase.Application.DTOs;
using Showcase.Application.Interfaces;
using Showcase.Domain.Entities;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = _userManager.Users.ToList();
        var result = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                Roles = roles
            });
        }
        return result;
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;
        var roles = await _userManager.GetRolesAsync(user);
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Roles = roles
        };
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
    {
        var user = new ApplicationUser
        {
            Email = dto.Email,
            UserName = dto.Email,
            DisplayName = dto.DisplayName
        };
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded) throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
        foreach (var role in dto.Roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));
        }
        await _userManager.AddToRolesAsync(user, dto.Roles);
        return await GetUserByIdAsync(user.Id) ?? throw new Exception("User creation failed");
    }

    public async Task<UserDto?> UpdateUserAsync(string userId, UpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;
        user.DisplayName = dto.DisplayName;
        await _userManager.UpdateAsync(user);

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        foreach (var role in dto.Roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));
        }
        await _userManager.AddToRolesAsync(user, dto.Roles);

        return await GetUserByIdAsync(userId);
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;
        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }
}
