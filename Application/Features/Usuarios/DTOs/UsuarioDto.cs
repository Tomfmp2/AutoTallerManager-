namespace Application.Features.Usuarios.DTOs;

public class UsuarioDto
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public int RoleId { get; set; }
    public string RoleNombre { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}