namespace Application.Features.Usuarios.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public int UserId { get; set; }
    public string Email { get; set; } = null!;
    public string RoleNombre { get; set; } = null!;
    public DateTime ExpirationTime { get; set; }
}