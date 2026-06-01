using Application.Abstractions;
using Application.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Application.Features.Appointments.Commands.ScheduleAppointment;

public class ScheduleAppointmentCommand : IRequest<Result<int>>
{
    [JsonIgnore]
    public int UserId { get; set; }

    public int VehicleId { get; set; }
    public int ServiceTypeId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string? Observations { get; set; }
}

public class ScheduleAppointmentCommandHandler : IRequestHandler<ScheduleAppointmentCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public ScheduleAppointmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(ScheduleAppointmentCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Person)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
            
        if (user == null) return Result<int>.Failure("Usuario no encontrado.");

        // Buscar el ID del cliente correspondiente a esta persona
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.PersonId == user.PersonId, cancellationToken);

        if (customer == null) return Result<int>.Failure("El usuario no está registrado como cliente.");

        // Validar vehículo
        var vehicle = await _context.Vehicles
            .Include(v => v.OwnerHistories)
            .FirstOrDefaultAsync(v => v.Id == request.VehicleId, cancellationToken);

        if (vehicle == null) return Result<int>.Failure("Vehículo no encontrado.");

        bool isOwner = vehicle.OwnerHistories.Any(h => h.CustomerId == customer.Id && !h.EndDate.HasValue);
        if (!isOwner) return Result<int>.Failure("El vehículo no pertenece a este usuario.");

        // Obtener ServiceType para duración
        var serviceType = await _context.ServiceTypes
            .FirstOrDefaultAsync(s => s.Id == request.ServiceTypeId, cancellationToken);

        if (serviceType == null) return Result<int>.Failure("Tipo de servicio no válido.");

        var startTime = request.ScheduledDate;
        var endTime = startTime.AddHours((double)(serviceType.EstimatedDurationHours ?? 1m));

        // Validar horario comercial
        var dayOfWeek = startTime.DayOfWeek;
        var startHour = startTime.TimeOfDay;
        var endHour = endTime.TimeOfDay;

        if (dayOfWeek == DayOfWeek.Sunday)
            return Result<int>.Failure("El taller está cerrado los domingos.");

        if (dayOfWeek == DayOfWeek.Saturday)
        {
            if (startHour < new TimeSpan(7, 0, 0) || endHour > new TimeSpan(12, 0, 0) || endTime.Date != startTime.Date)
                return Result<int>.Failure("El horario de los sábados es de 7:00 AM a 12:00 PM. El servicio excede este horario.");
        }
        else
        {
            if (startHour < new TimeSpan(7, 0, 0) || endHour > new TimeSpan(17, 0, 0) || endTime.Date != startTime.Date)
                return Result<int>.Failure("El horario de lunes a viernes es de 7:00 AM a 5:00 PM. El servicio excede este horario.");
        }

        var orderStatus = await _context.OrderStatuses
            .FirstOrDefaultAsync(os => os.Name == "Pendiente", cancellationToken);

        var workshopId = vehicle.WorkshopId;

        var newOrder = new ServiceOrder
        {
            WorkshopId = workshopId,
            VehicleId = request.VehicleId,
            ServiceTypeId = request.ServiceTypeId,
            MechanicId = null,
            OrderStatusId = orderStatus!.Id,
            ScheduledDate = request.ScheduledDate,
            Observations = request.Observations,
            EntryDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _context.ServiceOrders.Add(newOrder);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(newOrder.Id);
    }
}
