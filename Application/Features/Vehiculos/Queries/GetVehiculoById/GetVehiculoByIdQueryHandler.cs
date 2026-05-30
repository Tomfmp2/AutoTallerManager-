using Application.Abstractions;
using Application.Common;
using Application.Features.Vehiculos.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Vehiculos.Queries.GetVehiculoById;

public class GetVehiculoByIdQueryHandler : IRequestHandler<GetVehiculoByIdQuery, Result<VehiculoDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;

    public GetVehiculoByIdQueryHandler(IApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Result<VehiculoDto>> Handle(GetVehiculoByIdQuery request, CancellationToken cancellationToken)
    {
        var vehicle = await _db.Vehicles
            .Include(v => v.Model).ThenInclude(m => m!.Brand)
            .Include(v => v.Color)
            .Include(v => v.ServiceOrders)
            .Include(v => v.OwnerHistories)
                .ThenInclude(h => h.Customer)
                    .ThenInclude(c => c!.Person)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == request.Id && v.IsActive, cancellationToken);

        if (vehicle is null)
            return Result<VehiculoDto>.Failure($"No se encontró el vehículo con ID {request.Id}.");

        return Result<VehiculoDto>.Success(_mapper.Map<VehiculoDto>(vehicle));
    }
}
