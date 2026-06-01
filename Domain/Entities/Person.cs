using Domain.Common;

namespace Domain.Entities;

public class Person : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedAt { get; set; }

    public ICollection<PersonEmail> Emails { get; set; } = new List<PersonEmail>();
    public ICollection<PersonPhone> Phones { get; set; } = new List<PersonPhone>();
    public ICollection<PersonDocument> Documents { get; set; } = new List<PersonDocument>();
}
