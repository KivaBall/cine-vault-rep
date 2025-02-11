using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace CineVault.API.Entities;

public sealed class CineVaultDbContext(
    DbContextOptions<CineVaultDbContext> options,
    ILogger logger)
    : DbContext(options)
{
    public required DbSet<Movie> Movies { get; set; }
    public required DbSet<User> Users { get; set; }
    public required DbSet<Review> Reviews { get; set; }
    public required DbSet<Reaction> Reactions { get; set; }
    public required DbSet<Actor> Actors { get; set; }
    public required DbSet<MovieStat> MovieStats { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder
            .EnableSensitiveDataLogging()
            .LogTo(logger.Information, LogLevel.Information);
    }
}