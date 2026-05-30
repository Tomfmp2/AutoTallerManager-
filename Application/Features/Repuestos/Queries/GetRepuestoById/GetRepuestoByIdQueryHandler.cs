using Application.Abstractions;
using Application.Common;
using Application.Features.Repuestos.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Repuestos.Queries.GetRepuestoById;

public class GetRepuestoByIdQueryHandler : IRequestHandler<GetRepuestoByIdQuery, Result<RepuestoDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;

    public GetRepuestoByIdQueryHandler(IApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Result<RepuestoDto>> Handle(GetRepuestoByIdQuery request, CancellationToken cancellationToken)
    {
        var part = await _db.Parts
            .Include(p => p.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id && p.IsActive, cancellationToken);

        if (part is null)
            return Result<RepuestoDto>.Failure($"No se encontró el repuesto con ID {request.Id}.");

        return Result<RepuestoDto>.Success(_mapper.Map<RepuestoDto>(part));
    }
}
