using Application.Abstractions;
using Application.Common;
using Application.Features.Usuarios.DTOs;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Usuarios.Commands.GoogleLogin;

public class GoogleLoginCommand : IRequest<Result<LoginResponseDto>>
{
    public string IdToken { get; set; } = string.Empty;
}

public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, Result<LoginResponseDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IJwtService _jwtService;
    private readonly IGoogleAuthService _googleAuthService;

    public GoogleLoginCommandHandler(IApplicationDbContext db, IJwtService jwtService, IGoogleAuthService googleAuthService)
    {
        _db = db;
        _jwtService = jwtService;
        _googleAuthService = googleAuthService;
    }

    public async Task<Result<LoginResponseDto>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar Token con Google
        var googleUser = await _googleAuthService.ValidateGoogleTokenAsync(request.IdToken);
        if (googleUser == null)
            return Result<LoginResponseDto>.Failure("Token de Google inválido o expirado.");

        // 2. Buscar si el usuario ya existe por email
        var user = await _db.Users
            .Include(u => u.Person)
                .ThenInclude(p => p!.Emails)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.IsActive)
            .FirstOrDefaultAsync(u => u.Person != null && u.Person.Emails.Any(e => e.EmailUser + "@" + e.EmailDomain!.Domain == googleUser.Email), cancellationToken);

        // 3. Retornar error si el usuario no existe para que se registre manualmente
        if (user == null)
        {
            return Result<LoginResponseDto>.Failure($"Correo no registrado|{googleUser.Email}|{googleUser.FirstName}|{googleUser.LastName}");
        }

        // 4. Generar Tokens
        var roleName = user.UserRoles.FirstOrDefault()?.Role?.Name ?? "User";
        var token = _jwtService.GenerateToken(user, roleName);
        var refreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        user.LastLoginDate = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        return Result<LoginResponseDto>.Success(new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            Email = googleUser.Email,
            RoleNombre = roleName,
            ExpirationTime = DateTime.UtcNow.AddMinutes(60)
        });
    }
}
