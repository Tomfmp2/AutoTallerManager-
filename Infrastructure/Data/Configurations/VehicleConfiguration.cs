using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehiculos");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).HasColumnName("IdVehiculo");

        builder.Property(v => v.WorkshopId).HasColumnName("IdTaller").IsRequired();
        builder.Property(v => v.ModelId).HasColumnName("IdModelo").IsRequired();
        builder.Property(v => v.ColorId).HasColumnName("IdColor").IsRequired();

        builder.Property(v => v.LicensePlate).HasColumnName("Placa").HasMaxLength(15);
        builder.Property(v => v.VIN).HasColumnName("VIN").HasMaxLength(50).IsRequired();
        builder.Property(v => v.Year).HasColumnName("Anio");
        builder.Property(v => v.Mileage).HasColumnName("Kilometraje").HasDefaultValue(0);
        builder.Property(v => v.FuelType).HasColumnName("TipoCombustible").HasMaxLength(30);
        builder.Property(v => v.BodyType).HasColumnName("TipoCarroceria").HasMaxLength(30);
        builder.Property(v => v.EngineNumber).HasColumnName("NumeroMotor").HasMaxLength(50);
        builder.Property(v => v.Notes).HasColumnName("Notas").HasMaxLength(500);

        builder.Property(v => v.IsActive).HasColumnName("Activo").HasDefaultValue(true);
        builder.Property(v => v.CreatedAt).HasColumnName("FechaCreacion");
        builder.Property(v => v.LastModifiedAt).HasColumnName("FechaModificacion");

        // Relaciones
        builder.HasOne(v => v.Workshop)
            .WithMany()
            .HasForeignKey(v => v.WorkshopId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.Model)
            .WithMany()
            .HasForeignKey(v => v.ModelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.Color)
            .WithMany()
            .HasForeignKey(v => v.ColorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(v => new { v.WorkshopId, v.VIN }).IsUnique();

        builder.HasIndex(v => new { v.WorkshopId, v.LicensePlate }).IsUnique().HasFilter("\"Placa\" IS NOT NULL");

        builder.HasQueryFilter(v => v.IsActive);
    }
}