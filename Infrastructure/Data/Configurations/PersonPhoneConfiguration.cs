using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class PersonPhoneConfiguration : IEntityTypeConfiguration<PersonPhone>
{
    public void Configure(EntityTypeBuilder<PersonPhone> builder)
    {
        builder.ToTable("PersonasTelefonos");
        builder.HasKey(pp => pp.Id);
        builder.Property(pp => pp.Id).HasColumnName("IdPersonaTelefono");

        builder.Property(pp => pp.PersonId).HasColumnName("IdPersona").IsRequired();
        builder.Property(pp => pp.PhoneCodeId).HasColumnName("IdCodigoTelefono").IsRequired();
        builder.Property(pp => pp.PhoneNumber).HasColumnName("NumeroTelefono").HasMaxLength(20).IsRequired();
        builder.Property(pp => pp.IsPrimary).HasColumnName("EsPrincipal").HasDefaultValue(false);

        builder.HasOne(pp => pp.Person)
            .WithMany(p => p.Phones)
            .HasForeignKey(pp => pp.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pp => pp.PhoneCode)
            .WithMany()
            .HasForeignKey(pp => pp.PhoneCodeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(pp => new { pp.PersonId, pp.PhoneCodeId, pp.PhoneNumber }).IsUnique();
    }
}