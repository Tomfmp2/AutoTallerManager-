using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ServiceOrderConfiguration : IEntityTypeConfiguration<ServiceOrder>
{
    public void Configure(EntityTypeBuilder<ServiceOrder> builder)
    {
        builder.ToTable("OrdenesServicio");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasColumnName("IdOrdenServicio");

        builder.Property(o => o.WorkshopId).HasColumnName("IdTaller").IsRequired();
        builder.Property(o => o.VehicleId).HasColumnName("IdVehiculo").IsRequired();
        builder.Property(o => o.ServiceTypeId).HasColumnName("IdTipoServicio").IsRequired();
        builder.Property(o => o.MechanicId).HasColumnName("IdMecanico").IsRequired();
        builder.Property(o => o.ReceptionistId).HasColumnName("IdRecepcionista");
        builder.Property(o => o.OrderStatusId).HasColumnName("IdEstadoOrden").IsRequired();

        builder.Property(o => o.ScheduledDate).HasColumnName("FechaProgramada");
        builder.Property(o => o.EntryDate).HasColumnName("FechaIngreso");
        builder.Property(o => o.EstimatedDeliveryDate).HasColumnName("FechaEntregaEstimada");
        builder.Property(o => o.ActualDeliveryDate).HasColumnName("FechaEntregaReal");

        builder.Property(o => o.WorkPerformed).HasColumnName("TrabajoRealizado").HasColumnType("TEXT");
        builder.Property(o => o.Observations).HasColumnName("Observaciones").HasColumnType("TEXT");

        builder.Property(o => o.CreatedAt).HasColumnName("FechaCreacion");
        builder.Property(o => o.LastModifiedAt).HasColumnName("FechaModificacion");

        // Relaciones
        builder.HasOne(o => o.Workshop).WithMany().HasForeignKey(o => o.WorkshopId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(o => o.Vehicle).WithMany(v => v.ServiceOrders).HasForeignKey(o => o.VehicleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(o => o.ServiceType).WithMany().HasForeignKey(o => o.ServiceTypeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(o => o.Mechanic).WithMany().HasForeignKey(o => o.MechanicId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(o => o.Receptionist).WithMany().HasForeignKey(o => o.ReceptionistId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(o => o.OrderStatus).WithMany().HasForeignKey(o => o.OrderStatusId).OnDelete(DeleteBehavior.Restrict);
    }
}