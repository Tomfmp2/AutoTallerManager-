using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class VehicleOwnerHistoryConfiguration : IEntityTypeConfiguration<VehicleOwnerHistory>
{
    public void Configure(EntityTypeBuilder<VehicleOwnerHistory> builder)
    {
        builder.ToTable("HistorialDuenosVehiculo");
        builder.HasKey(vh => vh.Id);
        builder.Property(vh => vh.Id).HasColumnName("IdHistorial");

        builder.Property(vh => vh.VehicleId).HasColumnName("IdVehiculo").IsRequired();
        builder.Property(vh => vh.CustomerId).HasColumnName("IdCliente").IsRequired();

        builder.Property(vh => vh.StartDate).HasColumnName("FechaInicio").HasColumnType("DATE").IsRequired();
        builder.Property(vh => vh.EndDate).HasColumnName("FechaFin").HasColumnType("DATE");

        // Relaciones
        builder.HasOne(vh => vh.Vehicle)
            .WithMany(v => v.OwnerHistories)
            .HasForeignKey(vh => vh.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(vh => vh.Customer)
            .WithMany(c => c.VehicleOwnerHistories)
            .HasForeignKey(vh => vh.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}