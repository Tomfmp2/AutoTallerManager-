using Domain.Common;

namespace Domain.Entities;

public class VehicleColor : BaseEntity
{
    public string ColorName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}