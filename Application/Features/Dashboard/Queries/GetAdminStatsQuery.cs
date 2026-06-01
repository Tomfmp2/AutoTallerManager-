using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Application.Features.Dashboard.Queries;

public class AdminStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalVehicles { get; set; }
    public int TotalAppointments { get; set; }
    public decimal TotalRevenue { get; set; }
    
    public List<ChartData> AppointmentsByDay { get; set; } = new();
    public List<ChartData> AppointmentsByWeek { get; set; } = new();
    public List<ChartData> AppointmentsByMonth { get; set; } = new();
    public List<ChartData> AppointmentsByYear { get; set; } = new();
}

public class ChartData
{
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
}

public class GetAdminStatsQuery : IRequest<Result<AdminStatsDto>>
{
}

public class GetAdminStatsQueryHandler : IRequestHandler<GetAdminStatsQuery, Result<AdminStatsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAdminStatsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AdminStatsDto>> Handle(GetAdminStatsQuery request, CancellationToken cancellationToken)
    {
        var stats = new AdminStatsDto();
        
        stats.TotalUsers = await _context.Users.CountAsync(cancellationToken);
        stats.TotalVehicles = await _context.Vehicles.CountAsync(cancellationToken);
        stats.TotalAppointments = await _context.ServiceOrders.CountAsync(cancellationToken);
        stats.TotalRevenue = await _context.Invoices.SumAsync(f => f.Total, cancellationToken);
        
        // Citas (basado en ScheduledDate)
        var orders = await _context.ServiceOrders
            .Where(o => o.ScheduledDate.HasValue)
            .Select(o => o.ScheduledDate!.Value)
            .ToListAsync(cancellationToken);

        // Agrupar por mes del año actual
        var currentYear = DateTime.UtcNow.Year;
        var ordersThisYear = orders.Where(d => d.Year == currentYear).ToList();
        
        for (int i = 1; i <= 12; i++)
        {
            stats.AppointmentsByMonth.Add(new ChartData
            {
                Name = new DateTime(currentYear, i, 1).ToString("MMM"),
                Value = ordersThisYear.Count(d => d.Month == i)
            });
        }

        // Agrupar por día de la semana actual
        var today = DateTime.UtcNow.Date;
        var diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
        var startOfWeek = today.AddDays(-1 * diff);
        var ordersThisWeek = orders.Where(d => d.Date >= startOfWeek && d.Date < startOfWeek.AddDays(7)).ToList();

        var days = new[] { "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb", "Dom" };
        for (int i = 0; i < 7; i++)
        {
            var dayDate = startOfWeek.AddDays(i);
            stats.AppointmentsByDay.Add(new ChartData
            {
                Name = days[i],
                Value = ordersThisWeek.Count(d => d.Date == dayDate)
            });
        }

        // Agrupar por semanas del mes actual (simplificado 4-5 semanas)
        var currentMonth = DateTime.UtcNow.Month;
        var ordersThisMonth = orders.Where(d => d.Year == currentYear && d.Month == currentMonth).ToList();
        
        stats.AppointmentsByWeek = new List<ChartData>
        {
            new ChartData { Name = "Sem 1", Value = ordersThisMonth.Count(d => d.Day <= 7) },
            new ChartData { Name = "Sem 2", Value = ordersThisMonth.Count(d => d.Day > 7 && d.Day <= 14) },
            new ChartData { Name = "Sem 3", Value = ordersThisMonth.Count(d => d.Day > 14 && d.Day <= 21) },
            new ChartData { Name = "Sem 4", Value = ordersThisMonth.Count(d => d.Day > 21) }
        };

        // Agrupar por año (últimos 5 años)
        for (int i = 4; i >= 0; i--)
        {
            var year = currentYear - i;
            stats.AppointmentsByYear.Add(new ChartData
            {
                Name = year.ToString(),
                Value = orders.Count(d => d.Year == year)
            });
        }

        return Result<AdminStatsDto>.Success(stats);
    }
}
