using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class PersonEmailConfiguration : IEntityTypeConfiguration<PersonEmail>
{
    public void Configure(EntityTypeBuilder<PersonEmail> builder)
    {
        builder.ToTable("PersonasCorreos");
        builder.HasKey(pe => pe.Id);
        builder.Property(pe => pe.Id).HasColumnName("IdPersonaCorreo");

        builder.Property(pe => pe.PersonId).HasColumnName("IdPersona").IsRequired();
        builder.Property(pe => pe.EmailDomainId).HasColumnName("IdDominioCorreo").IsRequired();
        builder.Property(pe => pe.EmailUser).HasColumnName("UsuarioCorreo").HasMaxLength(100).IsRequired();
        builder.Property(pe => pe.IsPrimary).HasColumnName("EsPrincipal").HasDefaultValue(false);

        builder.HasOne(pe => pe.Person)
            .WithMany(p => p.Emails)
            .HasForeignKey(pe => pe.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pe => pe.EmailDomain)
            .WithMany()
            .HasForeignKey(pe => pe.EmailDomainId)
            .OnDelete(DeleteBehavior.Restrict);

        // Una persona no puede tener exactamente el mismo correo dos veces
        builder.HasIndex(pe => new { pe.PersonId, pe.EmailDomainId, pe.EmailUser }).IsUnique();
    }
}