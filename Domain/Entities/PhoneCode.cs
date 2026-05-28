using Domain.Common;    

namespace Domain.Entities;

public class PhoneCode : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}