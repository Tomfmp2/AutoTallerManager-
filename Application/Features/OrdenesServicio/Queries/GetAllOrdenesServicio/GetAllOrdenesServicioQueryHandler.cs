using Application.Abstractions;
using Application.Common;
using Application.Features.OrdenesServicio.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.OrdenesServicio.Queries.GetAllOrdenesServicio;

public class GetAllOrdenesServicioQueryHandler
    : IRequestHandler<GetAllOrdenesServicioQuery, Result<PagedResult<OrdenServicioDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;

    public GetAllOrdenesServicioQueryHandler(IApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<OrdenServicioDto>>> Handle(
        GetAllOrdenesServicioQuery request, CancellationToken cancellationToken)
    {
        var query = _db.ServiceOrders
            .Include(o => o.Vehicle)
                .ThenInclude(v => v!.OwnerHistories)
                    .ThenInclude(h => h.Customer)
                        .ThenInclude(c => c!.Person)
            .Include(o => o.ServiceType)
            .Include(o => o.Mechanic).ThenInclude(m => m!.Person)
            .Include(o => o.OrderStatus)
            .AsNoTracking();

        if (request.EstadoId.HasValue)
            query = query.Where(o => o.OrderStatusId == request.EstadoId.Value);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(o => o.EntryDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<OrdenServicioDto>>(items);
        return Result<PagedResult<OrdenServicioDto>>.Success(
            new PagedResult<OrdenServicioDto>(dtos, total, request.PageNumber, request.PageSize));
    }
}
