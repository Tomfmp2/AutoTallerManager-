using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ServiceOrderReportConfiguration : IEntityTypeConfiguration<ServiceOrderReport>
{
    public void Configure(EntityTypeBuilder<ServiceOrderReport> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.ReportText)
            .IsRequired()
            .HasMaxLength(2000);

        builder.HasOne(r => r.ServiceOrder)
            .WithMany()
            .HasForeignKey(r => r.ServiceOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Mechanic)
            .WithMany()
            .HasForeignKey(r => r.MechanicId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
