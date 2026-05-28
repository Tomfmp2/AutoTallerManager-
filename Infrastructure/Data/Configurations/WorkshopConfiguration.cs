using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class WorkshopConfiguration : IEntityTypeConfiguration<Workshop>
{
    public void Configure(EntityTypeBuilder<Workshop> builder)
    {
        builder.ToTable("Talleres");
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id).HasColumnName("IdTaller");

        builder.Property(w => w.Name).HasColumnName("Nombre").HasMaxLength(120).IsRequired();
        builder.Property(w => w.Nit).HasColumnName("Nit").HasMaxLength(20);
        builder.Property(w => w.BusinessName).HasColumnName("RazonSocial").HasMaxLength(150);
        builder.Property(w => w.Address).HasColumnName("Direccion").HasMaxLength(255);
        builder.Property(w => w.City).HasColumnName("Ciudad").HasMaxLength(80);
        builder.Property(w => w.Phone).HasColumnName("Telefono").HasMaxLength(20);
        builder.Property(w => w.Email).HasColumnName("Email").HasMaxLength(100);
        builder.Property(w => w.LogoUrl).HasColumnName("LogoUrl").HasMaxLength(255);

        builder.Property(w => w.IsActive).HasColumnName("Activo").HasDefaultValue(true);
        builder.Property(w => w.CreatedAt).HasColumnName("FechaCreacion");
        builder.Property(w => w.LastModifiedAt).HasColumnName("FechaModificacion");

        builder.HasIndex(w => w.Nit).IsUnique();
        builder.HasQueryFilter(w => w.IsActive);
    }
}