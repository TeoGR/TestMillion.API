using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestMillion.Domain.Entities;

namespace TestMillion.Persistence.Configuration;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("Properties");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).IsRequired();

        builder.Property(x => x.IdOwner)
               .IsRequired();

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Address).HasMaxLength(250);
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        builder.Property(x => x.CodeInternal).HasMaxLength(6);
        builder.Property(x => x.Year);
        builder.Property(x => x.Image).HasMaxLength(512);

        builder.HasIndex(x => x.CodeInternal).IsUnique(true);
    }
}
