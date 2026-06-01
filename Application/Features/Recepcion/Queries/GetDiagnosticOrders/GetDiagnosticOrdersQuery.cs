using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Features.OrdenesServicio.DTOs;
using AutoMapper;

namespace Application.Features.Recepcion.Queries.GetDiagnosticOrders;

public class GetDiagnosticOrdersQuery : IRequest<Result<List<OrdenServicioDto>>>
{
}

public class GetDiagnosticOrdersQueryHandler : IRequestHandler<GetDiagnosticOrdersQuery, Result<List<OrdenServicioDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDiagnosticOrdersQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<OrdenServicioDto>>> Handle(GetDiagnosticOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _context.ServiceOrders
            .Include(o => o.Vehicle)
                .ThenInclude(v => v!.OwnerHistories)
                    .ThenInclude(h => h.Customer)
                        .ThenInclude(c => c!.Person)
            .Include(o => o.ServiceType)
            .Include(o => o.OrderStatus)
            .Where(o => o.OrderStatus!.Name == "DiagnosticoCompletado")
            .OrderBy(o => o.EntryDate)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<OrdenServicioDto>>(orders);
        return Result<List<OrdenServicioDto>>.Success(dtos);
    }
}
