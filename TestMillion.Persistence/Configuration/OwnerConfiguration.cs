using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestMillion.Domain.Entities;

namespace TestMillion.Persistence.Configuration;

public class OwnerConfiguration : IEntityTypeConfiguration<Owner>
{
    public void Configure(EntityTypeBuilder<Owner> builder)
    {
        builder.ToTable("Owners");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .IsRequired();

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Address).HasMaxLength(250);
        builder.Property(x => x.Photo).HasMaxLength(512).IsRequired(false);
        builder.Property(x => x.Birthday);
    }
}
