using Application.Abstractions;
using Application.Common;
using Application.Features.Vehiculos.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Vehiculos.Queries.GetAllVehiculos;

public class GetAllVehiculosQueryHandler : IRequestHandler<GetAllVehiculosQuery, Result<PagedResult<VehiculoDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;

    public GetAllVehiculosQueryHandler(IApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<VehiculoDto>>> Handle(GetAllVehiculosQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Vehicles
            .Include(v => v.Model).ThenInclude(m => m!.Brand)
            .Include(v => v.Color)
            .Include(v => v.ServiceOrders)
            .Include(v => v.OwnerHistories)
                .ThenInclude(h => h.Customer)
                    .ThenInclude(c => c!.Person)
            .Where(v => v.IsActive)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLower();
            query = query.Where(v =>
                (v.LicensePlate != null && v.LicensePlate.ToLower().Contains(s)) ||
                v.VIN.ToLower().Contains(s));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(v => v.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<VehiculoDto>>(items);
        return Result<PagedResult<VehiculoDto>>.Success(
            new PagedResult<VehiculoDto>(dtos, total, request.PageNumber, request.PageSize));
    }
}
