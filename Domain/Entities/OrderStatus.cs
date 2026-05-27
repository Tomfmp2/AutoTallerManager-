using Domain.Common;

namespace Domain.Entities;

public class OrderStatus : BaseEntity
{
	public string Name { get; set; } = string.Empty;
	public bool IsFinal { get; set; } = false;
	public string? Description { get; set; }
}