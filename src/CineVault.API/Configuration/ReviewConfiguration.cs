using CineVault.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineVault.API.Configuration;

public sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.Property(m => m.Id)
            .HasColumnName("ReviewId");

        builder.HasQueryFilter(r => !r.IsDeleted);

        builder.Property(r => r.Comment)
            .HasMaxLength(512);
    }
}