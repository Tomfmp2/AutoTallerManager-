using Domain.Common;

namespace Domain.Entities;

public class VehicleModel : BaseEntity
{
    public int BrandId { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    //Navegacion
    public VehicleBrand? Brand { get; set; }

}
