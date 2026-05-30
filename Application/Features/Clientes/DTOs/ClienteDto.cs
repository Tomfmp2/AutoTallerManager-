namespace Application.Features.Clientes.DTOs;

public class ClienteDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Telefono { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public string? Direccion { get; set; }
    public int TotalVehiculos { get; set; }
    public DateTime CreatedAt { get; set; }
}