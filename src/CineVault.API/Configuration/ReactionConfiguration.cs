namespace CineVault.API.Configuration;

public sealed class ReactionConfiguration : IEntityTypeConfiguration<Reaction>
{
    public void Configure(EntityTypeBuilder<Reaction> builder)
    {
        // TODO 4 Зробити три зміни на власний розсуд в структурі бази даних та створити міграцію
        builder.HasKey(x => x.Id)
            .HasName("ReactionId");

        // TODO 10 Налаштувати фільтри на рівні DbContext, щоб виключати видалені записи з запитів
        builder.HasQueryFilter(r => !r.IsDeleted);

        builder
            .HasOne(r => r.User)
            .WithMany(u => u.Reactions)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // TODO 7 Забезпечення унікальних ключів
        builder.HasIndex(r => new { r.UserId, r.ReviewId })
            .IsUnique();
    }
}