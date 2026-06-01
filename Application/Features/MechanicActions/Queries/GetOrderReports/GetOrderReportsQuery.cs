using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.MechanicActions.Queries.GetOrderReports;

public class ServiceOrderReportDto
{
    public int Id { get; set; }
    public string ReportText { get; set; } = string.Empty;
    public bool IsDiagnostic { get; set; }
    public int? EstimatedHours { get; set; }
    public DateTime CreatedAt { get; set; }
    public string MechanicName { get; set; } = string.Empty;
}

public class GetOrderReportsQuery : IRequest<Result<List<ServiceOrderReportDto>>>
{
    public int ServiceOrderId { get; set; }
}

public class GetOrderReportsQueryHandler : IRequestHandler<GetOrderReportsQuery, Result<List<ServiceOrderReportDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetOrderReportsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ServiceOrderReportDto>>> Handle(GetOrderReportsQuery request, CancellationToken cancellationToken)
    {
        var reports = await _context.ServiceOrderReports
            .Include(r => r.Mechanic)
                .ThenInclude(m => m!.Person)
            .Where(r => r.ServiceOrderId == request.ServiceOrderId)
            .OrderBy(r => r.CreatedAt)
            .Select(r => new ServiceOrderReportDto
            {
                Id = r.Id,
                ReportText = r.ReportText,
                IsDiagnostic = r.IsDiagnostic,
                EstimatedHours = r.EstimatedHours,
                CreatedAt = r.CreatedAt,
                MechanicName = r.Mechanic!.Person!.FirstName + " " + r.Mechanic.Person.LastName
            })
            .ToListAsync(cancellationToken);

        return Result<List<ServiceOrderReportDto>>.Success(reports);
    }
}
