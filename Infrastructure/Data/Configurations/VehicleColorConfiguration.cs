using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class VehicleColorConfiguration : IEntityTypeConfiguration<VehicleColor>
{
    public void Configure(EntityTypeBuilder<VehicleColor> builder)
    {
        builder.ToTable("ColoresVehiculo");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).HasColumnName("IdColor");

        builder.Property(v => v.ColorName)
            .HasColumnName("NombreColor")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(v => v.IsActive).HasColumnName("Activo").HasDefaultValue(true);

        builder.HasIndex(v => v.ColorName).IsUnique();
        builder.HasQueryFilter(v => v.IsActive);
    }
}