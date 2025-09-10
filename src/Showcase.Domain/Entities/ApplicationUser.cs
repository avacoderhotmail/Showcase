using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Showcase.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        // Add domain-specific properties here
        public string? DisplayName { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }


}
