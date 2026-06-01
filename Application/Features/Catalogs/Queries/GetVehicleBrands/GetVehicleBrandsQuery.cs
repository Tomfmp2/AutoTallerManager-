using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;

namespace Application.Features.Catalogs.Queries.GetVehicleBrands;

public class VehicleBrandDto
{
    public int Id { get; set; }
    public string BrandName { get; set; } = string.Empty;
}

public class GetVehicleBrandsQuery : IRequest<List<VehicleBrandDto>>
{
}

public class GetVehicleBrandsQueryHandler : IRequestHandler<GetVehicleBrandsQuery, List<VehicleBrandDto>>
{
    private readonly IApplicationDbContext _context;

    public GetVehicleBrandsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<VehicleBrandDto>> Handle(GetVehicleBrandsQuery request, CancellationToken cancellationToken)
    {
        return await _context.VehicleBrands
            .OrderBy(b => b.BrandName)
            .Select(b => new VehicleBrandDto
            {
                Id = b.Id,
                BrandName = b.BrandName
            })
            .ToListAsync(cancellationToken);
    }
}
