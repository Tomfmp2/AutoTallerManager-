using Application.Abstractions;
using Application.Common;
using Application.Features.Repuestos.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Repuestos.Queries.GetAllRepuestos;

public class GetAllRepuestosQueryHandler : IRequestHandler<GetAllRepuestosQuery, Result<PagedResult<RepuestoDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;

    public GetAllRepuestosQueryHandler(IApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<RepuestoDto>>> Handle(GetAllRepuestosQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Parts
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLower();
            query = query.Where(p =>
                p.Code.ToLower().Contains(s) ||
                p.Description.ToLower().Contains(s));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(p => p.Code)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<RepuestoDto>>(items);
        return Result<PagedResult<RepuestoDto>>.Success(
            new PagedResult<RepuestoDto>(dtos, total, request.PageNumber, request.PageSize));
    }
}
