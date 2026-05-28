using Domain.Common;

namespace Domain.Entities;

public class VehicleBrand : BaseEntity
{
	public string BrandName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}