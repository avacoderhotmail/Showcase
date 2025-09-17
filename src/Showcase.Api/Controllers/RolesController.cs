using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // only admins should list roles
public class RolesController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RolesController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    [HttpGet("debug")]
    [Authorize]
    public IActionResult Debug()
    {
        return Ok(new
        {
            IsInRole_Admin = User.IsInRole("Admin"),
            Claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var roles = _roleManager.Roles.Select(r => r.Name).ToList();
        return Ok(roles);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] string roleName)
    {
        if (string.IsNullOrEmpty(roleName))
        {
            return BadRequest("Role name cannot be empty.");
        }

        // Check if the role already exists
        var roleExists = await _roleManager.RoleExistsAsync(roleName);

        if (!roleExists)
        {
            // If the role doesn't exist, create it
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (result.Succeeded)
            {
                return Ok($"Role '{roleName}' created successfully.");
            }
            else
            {
                // Handle errors
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { errors });
            }
        }

        return Conflict($"Role '{roleName}' already exists.");
    }
}
