using CineVault.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineVault.API.Configuration;

public sealed class MovieStatConfiguration : IEntityTypeConfiguration<MovieStat>
{
    public void Configure(EntityTypeBuilder<MovieStat> builder)
    {
        builder.HasKey(m => m.MovieId);

        builder.HasOne(m => m.Movie).WithOne(m => m.MovieStat)
            .HasForeignKey<MovieStat>(m => m.MovieId)
            .IsRequired();
    }
}