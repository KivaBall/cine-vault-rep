using CineVault.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineVault.API.Configuration;

public sealed class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.Property(m => m.Id)
            .HasColumnName("MovieId");

        builder.HasQueryFilter(m => !m.IsDeleted);

        builder.Property(m => m.Title)
            .HasMaxLength(64);

        builder.HasIndex(m => m.Title)
            .IsUnique();

        builder.Property(m => m.Description)
            .HasMaxLength(512);

        builder.Property(m => m.Genre)
            .HasMaxLength(64);

        builder.Property(m => m.Director)
            .HasMaxLength(64);
    }
}