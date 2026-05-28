using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Infrastructure.Data.Configurations;

public class EmailDomainConfiguration : IEntityTypeConfiguration<EmailDomain>
{
    public void Configure(EntityTypeBuilder<EmailDomain> builder)
    {
        builder.ToTable("DominiosCorreo");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("IdDominioCorreo");

        builder.Property(e => e.Domain)
            .HasColumnName("Dominio")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.IsActive).HasColumnName("Activo").HasDefaultValue(true);
        
        builder.HasIndex(e => e.Domain).IsUnique();
        builder.HasQueryFilter(e => e.IsActive);
    }
}