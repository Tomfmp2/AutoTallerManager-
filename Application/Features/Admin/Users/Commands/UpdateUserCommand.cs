using Application.Abstractions;
using Application.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Admin.Users.Commands;

public class UpdateUserCommand : IRequest<Result<bool>>
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? Phone { get; set; }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Person)
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (user == null) return Result<bool>.Failure("Usuario no encontrado.");

        // Check Phone Uniqueness (excluding the current user's person record)
        if (!string.IsNullOrWhiteSpace(request.Phone) && user.Person != null)
        {
            var phoneExists = await _context.Persons.IgnoreQueryFilters()
                .AnyAsync(p => p.Phone == request.Phone && p.Id != user.Person.Id, cancellationToken);
            if (phoneExists)
                return Result<bool>.Failure("telefono ya registrado");
        }

        user.IsActive = request.IsActive;

        if (user.Person != null)
        {
            user.Person.FirstName = request.FirstName;
            user.Person.LastName = request.LastName;
            user.Person.Phone = request.Phone;
        }

        // Update Role
        var newRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == request.Role, cancellationToken);
        if (newRole != null)
        {
            var userRole = user.UserRoles.FirstOrDefault();
            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
            }
            
            _context.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = newRole.Id
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
