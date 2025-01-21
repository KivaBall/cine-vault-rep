namespace CineVault.API.Entities;

public sealed class CineVaultDbContext(DbContextOptions<CineVaultDbContext> options)
    : DbContext(options)
{
    public required DbSet<Movie> Movies { get; set; }
    public required DbSet<User> Users { get; set; }
    public required DbSet<Review> Reviews { get; set; }
    public required DbSet<Reaction> Reactions { get; set; }
}