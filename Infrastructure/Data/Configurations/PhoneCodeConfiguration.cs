using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class PhoneCodeConfiguration : IEntityTypeConfiguration<PhoneCode>
{
    public void Configure(EntityTypeBuilder<PhoneCode> builder)
    {
        builder.ToTable("CodigosTelefono");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("IdCodigoTelefono");

        builder.Property(p => p.Code)
            .HasColumnName("Codigo")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(p => p.Country)
            .HasColumnName("Pais")
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(p => p.IsActive).HasColumnName("Activo").HasDefaultValue(true);

        builder.HasIndex(p => p.Code).IsUnique();
        builder.HasQueryFilter(p => p.IsActive);
    }
}