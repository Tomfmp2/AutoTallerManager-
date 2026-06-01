using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class VehiclePhotoConfiguration : IEntityTypeConfiguration<VehiclePhoto>
{
    public void Configure(EntityTypeBuilder<VehiclePhoto> builder)
    {
        builder.ToTable("FotosVehiculo");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("IdFoto");

        builder.Property(p => p.VehicleId).HasColumnName("IdVehiculo").IsRequired();
        builder.Property(p => p.PhotoData).HasColumnName("FotoDatos").IsRequired(); // base64
        builder.Property(p => p.Caption).HasColumnName("Descripcion").HasMaxLength(200);
        builder.Property(p => p.IsPrimary).HasColumnName("EsPrincipal").HasDefaultValue(false);
        builder.Property(p => p.CreatedAt).HasColumnName("FechaCreacion");

        builder.HasOne(p => p.Vehicle)
            .WithMany(v => v.Photos)
            .HasForeignKey(p => p.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
