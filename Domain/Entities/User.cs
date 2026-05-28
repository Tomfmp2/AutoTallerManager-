using Domain.Common;

namespace Domain.Entities;

public class User : BaseEntity
{
    public int WorkshopId { get; set; }
    public int PersonId { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedAt { get; set; }

    // Navegacion

    public Workshop? Workshop { get; set; }
    public Person? Person { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}