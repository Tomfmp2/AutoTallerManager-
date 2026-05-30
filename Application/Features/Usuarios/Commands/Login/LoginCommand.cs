using Application.Abstractions;
using Application.Common;
using Application.Features.Usuarios.DTOs;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Usuarios.Commands.Login;

public class LoginCommand : IRequest<Result<LoginResponseDto>>
{
    public string Email    { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(IApplicationDbContext db, IJwtService jwtService)
    {
        _db = db;
        _jwtService = jwtService;
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var emailParts = request.Email.Split('@');
        if (emailParts.Length != 2)
            return Result<LoginResponseDto>.Failure("Formato de correo inválido.");

        var user = await _db.Users
            .Include(u => u.Person)
                .ThenInclude(p => p!.Emails)
                    .ThenInclude(e => e.EmailDomain)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.IsActive)
            .FirstOrDefaultAsync(u =>
                u.Person != null &&
                u.Person.Emails.Any(e =>
                    e.EmailUser == emailParts[0] &&
                    e.EmailDomain != null &&
                    e.EmailDomain.Domain == emailParts[1]),
                cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Result<LoginResponseDto>.Failure("Credenciales inválidas.");

        var roleName = user.UserRoles.FirstOrDefault()?.Role?.Name ?? "User";
        var token = _jwtService.GenerateToken(user, roleName);
        var refreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        user.LastLoginDate = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        return Result<LoginResponseDto>.Success(new LoginResponseDto
        {
            Token          = token,
            RefreshToken   = refreshToken,
            Email          = request.Email,
            RoleNombre     = roleName,
            ExpirationTime = DateTime.UtcNow.AddMinutes(60)
        });
    }
}
