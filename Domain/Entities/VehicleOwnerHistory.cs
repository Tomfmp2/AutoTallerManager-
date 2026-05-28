using Domain.Common;

namespace Domain.Entities;

public class VehicleOwnerHistory : BaseEntity
{
    public int VehicleId { get; set; }
    public int CustomerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    //  Navegacion

    public Vehicle? Vehicle { get; set; }
    public Customer? Customer { get; set; }
}