using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions;

/// <summary>
/// Interfaz de solo lectura que expone los DbSets al Application layer
/// para queries con Include, sin romper la arquitectura hexagonal.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Customer> Customers { get; }
    DbSet<Person> Persons { get; }
    DbSet<PersonEmail> PersonEmails { get; }
    DbSet<PersonPhone> PersonPhones { get; }
    DbSet<EmailDomain> EmailDomains { get; }
    DbSet<PhoneCode> PhoneCodes { get; }
    DbSet<Vehicle> Vehicles { get; }
    DbSet<VehicleModel> VehicleModels { get; }
    DbSet<VehicleBrand> VehicleBrands { get; }
    DbSet<VehicleColor> VehicleColors { get; }
    DbSet<VehicleOwnerHistory> VehicleOwnerHistories { get; }
    DbSet<Part> Parts { get; }
    DbSet<PartCategory> PartCategories { get; }
    DbSet<ServiceOrder> ServiceOrders { get; }
    DbSet<ServiceOrderPart> ServiceOrderParts { get; }
    DbSet<ServiceType> ServiceTypes { get; }
    DbSet<OrderStatus> OrderStatuses { get; }
    DbSet<User> Users { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<Role> Roles { get; }
    DbSet<Invoice> Invoices { get; }
    DbSet<PaymentMethod> PaymentMethods { get; }
    DbSet<Workshop> Workshops { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
