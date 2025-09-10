using Showcase.Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public string CreatedByIp { get; set; } = "unknown";
    public bool Revoked { get; set; } = false;
    public DateTime? RevokedAt { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;
}
