using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Features.OrdenesServicio.DTOs;
using AutoMapper;

namespace Application.Features.MechanicActions.Queries.GetAssignedOrders;

public class GetAssignedOrdersQuery : IRequest<Result<List<OrdenServicioDto>>>
{
    public int MechanicId { get; set; }
}

public class GetAssignedOrdersQueryHandler : IRequestHandler<GetAssignedOrdersQuery, Result<List<OrdenServicioDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAssignedOrdersQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<OrdenServicioDto>>> Handle(GetAssignedOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _context.ServiceOrders
            .Include(o => o.Vehicle)
                .ThenInclude(v => v!.OwnerHistories)
                    .ThenInclude(h => h.Customer)
                        .ThenInclude(c => c!.Person)
            .Include(o => o.ServiceType)
            .Include(o => o.OrderStatus)
            .Include(o => o.Mechanic)
                .ThenInclude(m => m!.Person)
            .Where(o => o.MechanicId == request.MechanicId && 
                       (o.OrderStatus!.Name == "Programada" || 
                        o.OrderStatus.Name == "DiagnosticoCompletado" ||
                        o.OrderStatus.Name == "EnProceso"))
            .OrderBy(o => o.EntryDate)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<OrdenServicioDto>>(orders);
        return Result<List<OrdenServicioDto>>.Success(dtos);
    }
}
