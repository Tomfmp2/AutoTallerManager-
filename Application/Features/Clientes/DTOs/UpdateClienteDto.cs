namespace Application.Features.Clientes.DTOs;

public class UpdateClienteDto
{
    public string Nombre { get; set; } = null!;
    public string Telefono { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public string? Direccion { get; set; }
}