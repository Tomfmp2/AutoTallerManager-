using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Dashboard.Queries;

public class GetClientProfileQuery : IRequest<Result<ClientProfileDto>>
{
    public int UserId { get; set; }
}

public class ClientProfileDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? DateOfBirth { get; set; }
    public string? Role { get; set; }
    public string? MemberSince { get; set; }
}

public class GetClientProfileQueryHandler : IRequestHandler<GetClientProfileQuery, Result<ClientProfileDto>>
{
    private readonly IApplicationDbContext _db;

    public GetClientProfileQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<ClientProfileDto>> Handle(GetClientProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .Include(u => u.Person)
                .ThenInclude(p => p!.Emails)
                    .ThenInclude(e => e.EmailDomain)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == request.UserId && u.IsActive, cancellationToken);

        if (user?.Person == null)
            return Result<ClientProfileDto>.Failure("Perfil no encontrado.");

        var primaryEmail = user.Person.Emails
            .FirstOrDefault(e => e.IsPrimary)
            ?? user.Person.Emails.FirstOrDefault();

        var emailStr = primaryEmail != null
            ? $"{primaryEmail.EmailUser}@{primaryEmail.EmailDomain?.Domain}"
            : "Sin correo";

        var roleName = user.UserRoles.FirstOrDefault()?.Role?.Name ?? "Usuario";

        var dto = new ClientProfileDto
        {
            FirstName = user.Person.FirstName,
            LastName = user.Person.LastName,
            Email = emailStr,
            Phone = user.Person.Phone,
            DateOfBirth = user.Person.DateOfBirth?.ToString("dd/MM/yyyy"),
            Role = roleName,
            MemberSince = user.CreatedAt.ToString("MMMM yyyy")
        };

        return Result<ClientProfileDto>.Success(dto);
    }
}
