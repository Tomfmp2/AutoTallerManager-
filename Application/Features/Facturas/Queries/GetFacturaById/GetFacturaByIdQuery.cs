using Application.Abstractions;
using Application.Common;
using Application.Features.Facturas.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Facturas.Queries.GetFacturaById;

public class GetFacturaByIdQuery : IRequest<Result<FacturaDto>>
{
    public int Id { get; set; }
}

public class GetFacturaByIdQueryHandler : IRequestHandler<GetFacturaByIdQuery, Result<FacturaDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;

    public GetFacturaByIdQueryHandler(IApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Result<FacturaDto>> Handle(GetFacturaByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _db.Invoices
            .Include(i => i.Details)
            .Include(i => i.ServiceOrder)
                .ThenInclude(o => o!.Vehicle)
                    .ThenInclude(v => v!.OwnerHistories)
                        .ThenInclude(h => h.Customer)
                            .ThenInclude(c => c!.Person)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        if (invoice is null)
            return Result<FacturaDto>.Failure($"No se encontró la factura con ID {request.Id}.");

        return Result<FacturaDto>.Success(_mapper.Map<FacturaDto>(invoice));
    }
}
