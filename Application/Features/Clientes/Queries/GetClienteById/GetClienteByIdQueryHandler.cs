using Application.Abstractions;
using Application.Common;
using Application.Features.Clientes.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Clientes.Queries.GetClienteById;

public class GetClienteByIdQueryHandler : IRequestHandler<GetClienteByIdQuery, Result<ClienteDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;

    public GetClienteByIdQueryHandler(IApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Result<ClienteDto>> Handle(GetClienteByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _db.Customers
            .Include(c => c.Person)
                .ThenInclude(p => p!.Emails)
                    .ThenInclude(e => e.EmailDomain)
            .Include(c => c.Person)
                .ThenInclude(p => p!.Phones)
            .Include(c => c.VehicleOwnerHistories)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.IsActive, cancellationToken);

        if (customer is null)
            return Result<ClienteDto>.Failure($"No se encontró el cliente con ID {request.Id}.");

        return Result<ClienteDto>.Success(_mapper.Map<ClienteDto>(customer));
    }
}
