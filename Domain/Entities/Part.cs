using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities;

public class Part : BaseEntity
{
    public int WorkShopId { get; set; }
    public int PartCategoryId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? PartBrand { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? StorageLocation { get; set; }
    public int Stock { get; set; } = 0;
    public int MinimumStock { get; set; } = 0;
    public decimal UnitPrice { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedAt { get; set; }

    // Navegacion
    public Workshop? Workshop { get; set; }
    public PartCategory? Category { get; set; }
}