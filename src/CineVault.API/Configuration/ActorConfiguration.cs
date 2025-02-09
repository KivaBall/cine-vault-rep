using CineVault.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineVault.API.Configuration;

public sealed class ActorConfiguration : IEntityTypeConfiguration<Actor>
{
    public void Configure(EntityTypeBuilder<Actor> builder)
    {
        builder.Property(m => m.Id)
            .HasColumnName("ActorId");

        builder.HasQueryFilter(a => !a.IsDeleted);

        builder.Property(a => a.FullName)
            .HasMaxLength(64);

        builder.HasIndex(a => a.FullName)
            .IsUnique();

        builder.Property(a => a.Biography)
            .HasMaxLength(512);
    }
}