using Domain.Common;

namespace Domain.Entities;

public class EmailDomain : BaseEntity
{
    public string Domain { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}