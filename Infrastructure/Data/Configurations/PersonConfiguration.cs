using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("Personas");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("IdPersona");

        builder.Property(p => p.FirstName).HasColumnName("Nombres").HasMaxLength(100).IsRequired();
        builder.Property(p => p.LastName).HasColumnName("Apellidos").HasMaxLength(100).IsRequired();
        builder.Property(p => p.DateOfBirth).HasColumnName("FechaNacimiento").HasColumnType("DATE");
        builder.Property(p => p.Phone).HasColumnName("Telefono").HasMaxLength(50);

        builder.Property(p => p.IsActive).HasColumnName("Activo").HasDefaultValue(true);
        builder.Property(p => p.CreatedAt).HasColumnName("FechaCreacion");
        builder.Property(p => p.LastModifiedAt).HasColumnName("FechaModificacion");

        builder.HasQueryFilter(p => p.IsActive);
    }
}