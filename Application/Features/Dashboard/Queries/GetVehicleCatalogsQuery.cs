using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Dashboard.Queries;

public class GetVehicleCatalogsQuery : IRequest<Result<VehicleCatalogsDto>> { }

public class VehicleCatalogsDto
{
    public List<BrandDto> Brands { get; set; } = new();
    public List<ModelDto> Models { get; set; } = new();
    public List<ColorDto> Colors { get; set; } = new();
}

public class BrandDto  { public int Id { get; set; } public string Name { get; set; } = ""; }
public class ModelDto  { public int Id { get; set; } public int BrandId { get; set; } public string Name { get; set; } = ""; }
public class ColorDto  { public int Id { get; set; } public string Name { get; set; } = ""; }

public class GetVehicleCatalogsQueryHandler : IRequestHandler<GetVehicleCatalogsQuery, Result<VehicleCatalogsDto>>
{
    private readonly IApplicationDbContext _db;
    public GetVehicleCatalogsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<VehicleCatalogsDto>> Handle(GetVehicleCatalogsQuery request, CancellationToken ct)
    {
        var brands = await _db.VehicleBrands
            .Where(b => b.IsActive)
            .OrderBy(b => b.BrandName)
            .Select(b => new BrandDto { Id = b.Id, Name = b.BrandName })
            .ToListAsync(ct);

        var models = await _db.VehicleModels
            .Where(m => m.IsActive)
            .OrderBy(m => m.ModelName)
            .Select(m => new ModelDto { Id = m.Id, BrandId = m.BrandId, Name = m.ModelName })
            .ToListAsync(ct);

        var colors = await _db.VehicleColors
            .Where(c => c.IsActive)
            .OrderBy(c => c.ColorName)
            .Select(c => new ColorDto { Id = c.Id, Name = c.ColorName })
            .ToListAsync(ct);

        return Result<VehicleCatalogsDto>.Success(new VehicleCatalogsDto
        {
            Brands = brands,
            Models = models,
            Colors = colors
        });
    }
}
