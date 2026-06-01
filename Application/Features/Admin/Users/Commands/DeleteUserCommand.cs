using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Admin.Users.Commands;

public class DeleteUserCommand : IRequest<Result<bool>>
{
    public int Id { get; set; }
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Person)
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (user == null) return Result<bool>.Failure("Usuario no encontrado.");

        // 1. Delete User Roles
        var userRoles = await _context.UserRoles.IgnoreQueryFilters()
            .Where(ur => ur.UserId == user.Id)
            .ToListAsync(cancellationToken);
        _context.UserRoles.RemoveRange(userRoles);

        // 2. Delete Notifications
        var notifications = await _context.Notifications.IgnoreQueryFilters()
            .Where(n => n.UserId == user.Id)
            .ToListAsync(cancellationToken);
        _context.Notifications.RemoveRange(notifications);

        // 2.5 Delete Audits
        var audits = await _context.Audits.IgnoreQueryFilters()
            .Where(a => a.UserId == user.Id)
            .ToListAsync(cancellationToken);
        _context.Audits.RemoveRange(audits);

        // 3. Nullify or clean up Service Orders where this user was a Mechanic or Receptionist
        var assignedOrders = await _context.ServiceOrders.IgnoreQueryFilters()
            .Where(so => so.MechanicId == user.Id || so.ReceptionistId == user.Id)
            .ToListAsync(cancellationToken);
        foreach (var order in assignedOrders)
        {
            if (order.MechanicId == user.Id) order.MechanicId = null;
            if (order.ReceptionistId == user.Id) order.ReceptionistId = null;
        }

        // 3.5 Delete Service Order Reports where this user was a Mechanic
        var mechanicReports = await _context.ServiceOrderReports.IgnoreQueryFilters()
            .Where(sor => sor.MechanicId == user.Id)
            .ToListAsync(cancellationToken);
            
        if (mechanicReports.Any())
        {
            var mechanicReportIds = mechanicReports.Select(r => r.Id).ToList();
            var mechanicReportParts = await _context.ServiceOrderReportParts.IgnoreQueryFilters()
                .Where(sop => mechanicReportIds.Contains(sop.ServiceOrderReportId))
                .ToListAsync(cancellationToken);
            _context.ServiceOrderReportParts.RemoveRange(mechanicReportParts);
            _context.ServiceOrderReports.RemoveRange(mechanicReports);
        }

        // 4. Handle Customer and Vehicle data if the person is a Client
        if (user.Person != null)
        {
            var customers = await _context.Customers.IgnoreQueryFilters()
                .Where(c => c.PersonId == user.Person.Id)
                .ToListAsync(cancellationToken);

            foreach (var customer in customers)
            {
                // Find all Owner Histories for this Customer
                var ownerHistories = await _context.VehicleOwnerHistories.IgnoreQueryFilters()
                    .Where(oh => oh.CustomerId == customer.Id)
                    .ToListAsync(cancellationToken);

                // Find all vehicles associated with these histories
                var vehicleIds = ownerHistories.Select(oh => oh.VehicleId).Distinct().ToList();
                if (vehicleIds.Any())
                {
                    var vehicles = await _context.Vehicles.IgnoreQueryFilters()
                        .Where(v => vehicleIds.Contains(v.Id))
                        .ToListAsync(cancellationToken);

                    foreach (var vehicle in vehicles)
                    {
                        // Delete Vehicle Photos
                        var photos = await _context.VehiclePhotos.IgnoreQueryFilters()
                            .Where(vp => vp.VehicleId == vehicle.Id)
                            .ToListAsync(cancellationToken);
                        _context.VehiclePhotos.RemoveRange(photos);

                        // Find Service Orders for the vehicle
                        var serviceOrders = await _context.ServiceOrders.IgnoreQueryFilters()
                            .Where(so => so.VehicleId == vehicle.Id)
                            .ToListAsync(cancellationToken);

                        var orderIds = serviceOrders.Select(so => so.Id).ToList();
                        if (orderIds.Any())
                        {
                            // Delete Service Order Report Parts
                            var reportParts = await _context.ServiceOrderReportParts.IgnoreQueryFilters()
                                .Where(sop => _context.ServiceOrderReports.IgnoreQueryFilters()
                                    .Where(sor => orderIds.Contains(sor.ServiceOrderId))
                                    .Select(sor => sor.Id).Contains(sop.ServiceOrderReportId))
                                .ToListAsync(cancellationToken);
                            _context.ServiceOrderReportParts.RemoveRange(reportParts);

                            // Delete Service Order Reports
                            var reports = await _context.ServiceOrderReports.IgnoreQueryFilters()
                                .Where(sor => orderIds.Contains(sor.ServiceOrderId))
                                .ToListAsync(cancellationToken);
                            _context.ServiceOrderReports.RemoveRange(reports);

                            // Delete Service Order Parts
                            var orderParts = await _context.ServiceOrderParts.IgnoreQueryFilters()
                                .Where(sop => orderIds.Contains(sop.ServiceOrderId))
                                .ToListAsync(cancellationToken);
                            _context.ServiceOrderParts.RemoveRange(orderParts);

                            // Delete Invoices (including loading Details for EF cascade)
                            var invoices = await _context.Invoices.IgnoreQueryFilters()
                                .Include(i => i.Details)
                                .Where(i => orderIds.Contains(i.ServiceOrderId))
                                .ToListAsync(cancellationToken);
                            
                            var invoiceDetails = invoices.SelectMany(i => i.Details).ToList();
                            if (invoiceDetails.Any())
                            {
                                _context.InvoiceDetails.RemoveRange(invoiceDetails);
                            }
                            
                            _context.Invoices.RemoveRange(invoices);

                            // Delete Service Orders
                            _context.ServiceOrders.RemoveRange(serviceOrders);
                        }

                        // Delete all owner histories for this vehicle
                        var vehicleHistories = await _context.VehicleOwnerHistories.IgnoreQueryFilters()
                            .Where(oh => oh.VehicleId == vehicle.Id)
                            .ToListAsync(cancellationToken);
                        _context.VehicleOwnerHistories.RemoveRange(vehicleHistories);

                        // Delete the vehicle itself
                        _context.Vehicles.Remove(vehicle);
                    }
                }

                // Delete owner histories directly associated with the customer
                _context.VehicleOwnerHistories.RemoveRange(ownerHistories);

                // Delete Customer record
                _context.Customers.Remove(customer);
            }

            // 5. Delete Person related entities
            var emails = await _context.PersonEmails.IgnoreQueryFilters()
                .Where(pe => pe.PersonId == user.Person.Id)
                .ToListAsync(cancellationToken);
            _context.PersonEmails.RemoveRange(emails);

            var phones = await _context.PersonPhones.IgnoreQueryFilters()
                .Where(pp => pp.PersonId == user.Person.Id)
                .ToListAsync(cancellationToken);
            _context.PersonPhones.RemoveRange(phones);

            var documents = await _context.PersonDocuments.IgnoreQueryFilters()
                .Where(pd => pd.PersonId == user.Person.Id)
                .ToListAsync(cancellationToken);
            _context.PersonDocuments.RemoveRange(documents);

            // Delete User first so that EF Core doesn't complain about severing the required relationship with Person
            _context.Users.Remove(user);

            // Delete person itself
            _context.Persons.Remove(user.Person);
        }
        else
        {
            // 6. Delete User itself if person is null
            _context.Users.Remove(user);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
