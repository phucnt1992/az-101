using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Az101.Gallery.Models;

namespace Az101.Gallery.Infrastructure.Persistence.Configurations;

internal class PhotoTypeConfiguration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        builder.ToTable("Photos");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(m => m.Alt)
            .HasMaxLength(300);

        builder.Property(m => m.FileName)
            .HasMaxLength(1000);

        builder.Property(m => m.CreatedAt)
            .HasDefaultValueSql("getutcdate()");

        builder.Property(m => m.UpdateAt)
            .HasDefaultValueSql("getutcdate()");
    }
}

