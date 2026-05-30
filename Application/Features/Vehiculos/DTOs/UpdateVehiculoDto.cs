namespace Application.Features.Vehiculos.DTOs;

public class UpdateVehiculoDto
{
    public string Placa { get; set; } = null!;
    public string VIN { get; set; } = null!;
    public int Año { get; set; }
    public int Kilometraje { get; set; }
    public int VehicleModelId { get; set; }
    public int VehicleColorId { get; set; }
}