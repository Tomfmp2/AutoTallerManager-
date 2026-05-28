using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class VehicleModelConfiguration : IEntityTypeConfiguration<VehicleModel>
{
    public void Configure(EntityTypeBuilder<VehicleModel> builder)
    {
        builder.ToTable("ModelosVehiculo");
        builder.HasKey(vm => vm.Id);
        builder.Property(vm => vm.Id).HasColumnName("IdModelo");

        builder.Property(vm => vm.BrandId).HasColumnName("IdMarca").IsRequired();
        builder.Property(vm => vm.ModelName).HasColumnName("NombreModelo").HasMaxLength(80).IsRequired();

        builder.Property(vm => vm.IsActive).HasColumnName("Activo").HasDefaultValue(true);

        builder.HasOne(vm => vm.Brand)
            .WithMany()
            .HasForeignKey(vm => vm.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(vm => new { vm.BrandId, vm.ModelName }).IsUnique();
        builder.HasQueryFilter(vm => vm.IsActive);
    }
}