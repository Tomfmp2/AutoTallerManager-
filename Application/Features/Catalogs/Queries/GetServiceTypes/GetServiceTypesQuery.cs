using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;

namespace Application.Features.Catalogs.Queries.GetServiceTypes;

public class ServiceTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal EstimatedDurationHours { get; set; }
    public decimal PricePerHour { get; set; }
}

public class GetServiceTypesQuery : IRequest<List<ServiceTypeDto>>
{
}

public class GetServiceTypesQueryHandler : IRequestHandler<GetServiceTypesQuery, List<ServiceTypeDto>>
{
    private readonly IApplicationDbContext _context;

    public GetServiceTypesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ServiceTypeDto>> Handle(GetServiceTypesQuery request, CancellationToken cancellationToken)
    {
        return await _context.ServiceTypes
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .Select(s => new ServiceTypeDto
            {
                Id = s.Id,
                Name = s.Name,
                EstimatedDurationHours = s.EstimatedDurationHours ?? 1m,
                PricePerHour = s.PricePerHour
            })
            .ToListAsync(cancellationToken);
    }
}
