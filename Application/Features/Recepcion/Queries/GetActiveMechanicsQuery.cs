using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Application.Features.Recepcion.Queries;

public class GetActiveMechanicsQuery : IRequest<Result<List<MechanicDto>>>
{
}

public class MechanicDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class GetActiveMechanicsQueryHandler : IRequestHandler<GetActiveMechanicsQuery, Result<List<MechanicDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetActiveMechanicsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<MechanicDto>>> Handle(GetActiveMechanicsQuery request, CancellationToken cancellationToken)
    {
        var mechanics = await _context.Users
            .Include(u => u.Person)
                .ThenInclude(p => p.Emails)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.IsActive && u.UserRoles.Any(ur => ur.Role.Name == "Mecanico"))
            .Select(u => new MechanicDto
            {
                Id = u.Id,
                Name = (u.Person != null) ? u.Person.FirstName + " " + u.Person.LastName : "Mecánico",
                Email = ""
            })
            .ToListAsync(cancellationToken);

        return Result<List<MechanicDto>>.Success(mechanics);
    }
}
