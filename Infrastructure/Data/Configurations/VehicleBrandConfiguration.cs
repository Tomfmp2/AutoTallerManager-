using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class VehicleBrandConfiguration : IEntityTypeConfiguration<VehicleBrand>
{
    public void Configure(EntityTypeBuilder<VehicleBrand> builder)
    {
        builder.ToTable("MarcasVehiculo");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).HasColumnName("IdMarca");

        builder.Property(v => v.BrandName)
            .HasColumnName("NombreMarca")
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(v => v.IsActive).HasColumnName("Activo").HasDefaultValue(true);

        builder.HasIndex(v => v.BrandName).IsUnique();
        builder.HasQueryFilter(v => v.IsActive);
    }
}