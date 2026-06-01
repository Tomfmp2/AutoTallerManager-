using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;

namespace Application.Features.Catalogs.Queries.GetVehicleModels;

public class VehicleModelDto
{
    public int Id { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public int BrandId { get; set; }
}

public class GetVehicleModelsQuery : IRequest<List<VehicleModelDto>>
{
    public int? BrandId { get; set; }
}

public class GetVehicleModelsQueryHandler : IRequestHandler<GetVehicleModelsQuery, List<VehicleModelDto>>
{
    private readonly IApplicationDbContext _context;

    public GetVehicleModelsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<VehicleModelDto>> Handle(GetVehicleModelsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.VehicleModels.AsQueryable();
        
        if (request.BrandId.HasValue)
        {
            query = query.Where(m => m.BrandId == request.BrandId.Value);
        }

        return await query
            .OrderBy(m => m.ModelName)
            .Select(m => new VehicleModelDto
            {
                Id = m.Id,
                ModelName = m.ModelName,
                BrandId = m.BrandId
            })
            .ToListAsync(cancellationToken);
    }
}
