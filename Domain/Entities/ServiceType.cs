using Domain.Common;

namespace Domain.Entities;

public class ServiceType : BaseEntity
{
	public string Name { get; set; } = string.Empty;
	public decimal? EstimatedDurationHours { get; set; }
	public decimal PricePerHour { get; set; } = 0;
	public bool IsActive { get; set; } = true;
}