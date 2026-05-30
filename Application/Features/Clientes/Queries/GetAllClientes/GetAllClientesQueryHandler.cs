using Application.Abstractions;
using Application.Common;
using Application.Features.Clientes.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Clientes.Queries.GetAllClientes;

public class GetAllClientesQueryHandler : IRequestHandler<GetAllClientesQuery, Result<PagedResult<ClienteDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;

    public GetAllClientesQueryHandler(IApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<ClienteDto>>> Handle(GetAllClientesQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Customers
            .Include(c => c.Person)
                .ThenInclude(p => p!.Emails)
                    .ThenInclude(e => e.EmailDomain)
            .Include(c => c.Person)
                .ThenInclude(p => p!.Phones)
            .Include(c => c.VehicleOwnerHistories)
            .Where(c => c.IsActive)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(c =>
                c.Person != null &&
                (c.Person.FirstName.ToLower().Contains(search) ||
                 c.Person.LastName.ToLower().Contains(search)));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.Person!.LastName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<ClienteDto>>(items);
        return Result<PagedResult<ClienteDto>>.Success(
            new PagedResult<ClienteDto>(dtos, total, request.PageNumber, request.PageSize));
    }
}
