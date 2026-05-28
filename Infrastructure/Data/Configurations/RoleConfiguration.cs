using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("IdRol");

        builder.Property(r => r.Name).HasColumnName("Nombre").HasMaxLength(50).IsRequired();
        builder.Property(r => r.Description).HasColumnName("Descripcion").HasMaxLength(200);
        builder.Property(r => r.IsActive).HasColumnName("Activo").HasDefaultValue(true);

        builder.HasIndex(r => r.Name).IsUnique();
        builder.HasQueryFilter(r => r.IsActive);
    }
}