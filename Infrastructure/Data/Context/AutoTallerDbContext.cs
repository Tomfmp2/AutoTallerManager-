using System.Reflection;
using Application.Abstractions;
using Domain.Entities;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Context;

public class AutoTallerDbContext : DbContext, IApplicationDbContext
{
    public AutoTallerDbContext(DbContextOptions<AutoTallerDbContext> options) : base(options)
    {
    }
    public DbSet<DocumentType> DocumentTypes => Set<DocumentType>();
    public DbSet<EmailDomain> EmailDomains => Set<EmailDomain>();
    public DbSet<PhoneCode> PhoneCodes => Set<PhoneCode>();
    public DbSet<VehicleBrand> VehicleBrands => Set<VehicleBrand>();
    public DbSet<VehicleColor> VehicleColors => Set<VehicleColor>();
    public DbSet<PartCategory> PartCategories => Set<PartCategory>();
    public DbSet<ServiceType> ServiceTypes => Set<ServiceType>();
    public DbSet<OrderStatus> OrderStatuses => Set<OrderStatus>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<AuditActionType> AuditActionTypes => Set<AuditActionType>();

    public DbSet<Workshop> Workshops => Set<Workshop>();
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<PersonDocument> PersonDocuments => Set<PersonDocument>();
    public DbSet<PersonEmail> PersonEmails => Set<PersonEmail>(); 
    public DbSet<PersonPhone> PersonPhones => Set<PersonPhone>();

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<VehicleModel> VehicleModels => Set<VehicleModel>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<VehicleOwnerHistory> VehicleOwnerHistories => Set<VehicleOwnerHistory>();

    public DbSet<Part> Parts => Set<Part>();
    public DbSet<ServiceOrder> ServiceOrders => Set<ServiceOrder>();
    public DbSet<ServiceOrderPart> ServiceOrderParts => Set<ServiceOrderPart>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceDetail> InvoiceDetails => Set<InvoiceDetail>();
    public DbSet<Audit> Audits => Set<Audit>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    
                    var createdAtProp = entry.Entity.GetType().GetProperty("CreatedAt");
                    if (createdAtProp != null && createdAtProp.CanWrite)
                    {
                        createdAtProp.SetValue(entry.Entity, DateTime.UtcNow);
                    }
                    break;

                case EntityState.Modified:
                    var lastModProp = entry.Entity.GetType().GetProperty("LastModifiedAt");
                    if (lastModProp != null && lastModProp.CanWrite)
                    {
                        lastModProp.SetValue(entry.Entity, DateTime.UtcNow);
                    }
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

}
