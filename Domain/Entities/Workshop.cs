using Domain.Common;

namespace Domain.Entities;

public class Workshop : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Nit { get; set; }
    public string? BusinessName { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }   
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedAt { get; set; }
}