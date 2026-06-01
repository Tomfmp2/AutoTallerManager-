using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;

namespace Application.Features.Catalogs.Queries.GetVehicleColors;

public class VehicleColorDto
{
    public int Id { get; set; }
    public string ColorName { get; set; } = string.Empty;
}

public class GetVehicleColorsQuery : IRequest<List<VehicleColorDto>>
{
}

public class GetVehicleColorsQueryHandler : IRequestHandler<GetVehicleColorsQuery, List<VehicleColorDto>>
{
    private readonly IApplicationDbContext _context;

    public GetVehicleColorsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<VehicleColorDto>> Handle(GetVehicleColorsQuery request, CancellationToken cancellationToken)
    {
        return await _context.VehicleColors
            .OrderBy(c => c.ColorName)
            .Select(c => new VehicleColorDto
            {
                Id = c.Id,
                ColorName = c.ColorName
            })
            .ToListAsync(cancellationToken);
    }
}
