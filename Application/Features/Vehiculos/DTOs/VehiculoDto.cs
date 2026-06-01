namespace Application.Features.Vehiculos.DTOs;

public class VehiculoDto
{
    public int Id { get; set; }
    public string Placa { get; set; } = null!;
    public string VIN { get; set; } = null!;
    public int Anio { get; set; }
    public int Kilometraje { get; set; }
    public string Marca { get; set; } = null!;
    public string Modelo { get; set; } = null!;
    public string Color { get; set; } = null!;
    public string? PropietarioActual { get; set; }
    public int? PropietarioId { get; set; }
    public int TotalOrdenesServicio { get; set; }
    public DateTime CreatedAt { get; set; }
}