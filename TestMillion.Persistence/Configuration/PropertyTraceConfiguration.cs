using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestMillion.Domain.Entities;

namespace TestMillion.Persistence.Configuration;

public class PropertyTraceConfiguration : IEntityTypeConfiguration<PropertyTrace>
{
    public void Configure(EntityTypeBuilder<PropertyTrace> builder)
    {
        builder.ToTable("PropertyTraces");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DateSale).IsRequired();
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        builder.HasOne<Property>()
               .WithMany()
               .HasForeignKey(x => x.PropertyId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
