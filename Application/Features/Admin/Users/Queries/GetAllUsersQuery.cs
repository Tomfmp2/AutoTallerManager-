using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Admin.Users.Queries;

public class UserAdminDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class GetAllUsersQuery : IRequest<Result<List<UserAdminDto>>>
{
    public string? SearchTerm { get; set; }
}

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<List<UserAdminDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<UserAdminDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users
            .Include(u => u.Person)
                .ThenInclude(p => p.Emails)
                    .ThenInclude(e => e.EmailDomain)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(u => 
                (u.Person != null && u.Person.Emails.Any(e => (e.EmailUser + "@" + e.EmailDomain.Domain).ToLower().Contains(term))) || 
                u.Id.ToString() == term ||
                (u.Person != null && (u.Person.FirstName.ToLower().Contains(term) || u.Person.LastName.ToLower().Contains(term)))
            );
        }

        var users = await query.ToListAsync(cancellationToken);
        
        var dtoList = users.Select(u => {
            var firstEmail = u.Person?.Emails?.FirstOrDefault();
            var emailStr = firstEmail != null ? $"{firstEmail.EmailUser}@{firstEmail.EmailDomain?.Domain}" : "";
            
            return new UserAdminDto
            {
                Id = u.Id,
                Email = emailStr,
                IsActive = u.IsActive,
                Role = u.UserRoles.FirstOrDefault()?.Role?.Name ?? "",
                FirstName = u.Person != null ? u.Person.FirstName : "",
                LastName = u.Person != null ? u.Person.LastName : "",
                Phone = u.Person != null ? u.Person.Phone ?? "" : ""
            };
        }).ToList();

        return Result<List<UserAdminDto>>.Success(dtoList);
    }
}
