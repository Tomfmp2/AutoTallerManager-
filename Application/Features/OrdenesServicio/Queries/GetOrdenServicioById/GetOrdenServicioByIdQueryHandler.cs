using Application.Abstractions;
using Application.Common;
using Application.Features.OrdenesServicio.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.OrdenesServicio.Queries.GetOrdenServicioById;

public class GetOrdenServicioByIdQueryHandler
    : IRequestHandler<GetOrdenServicioByIdQuery, Result<OrdenServicioDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;

    public GetOrdenServicioByIdQueryHandler(IApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Result<OrdenServicioDto>> Handle(
        GetOrdenServicioByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _db.ServiceOrders
            .Include(o => o.Vehicle)
                .ThenInclude(v => v!.OwnerHistories)
                    .ThenInclude(h => h.Customer)
                        .ThenInclude(c => c!.Person)
            .Include(o => o.ServiceType)
            .Include(o => o.Mechanic).ThenInclude(m => m!.Person)
            .Include(o => o.OrderStatus)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order is null)
            return Result<OrdenServicioDto>.Failure($"No se encontró la orden de servicio con ID {request.Id}.");

        return Result<OrdenServicioDto>.Success(_mapper.Map<OrdenServicioDto>(order));
    }
}
