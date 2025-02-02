namespace CineVault.API.Configuration;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // TODO 4 Зробити три зміни на власний розсуд в структурі бази даних та створити міграцію
        builder.Property(m => m.Id)
            .HasColumnName("UserId");

        // TODO 10 Налаштувати фільтри на рівні DbContext, щоб виключати видалені записи з запитів
        builder.HasQueryFilter(u => !u.IsDeleted);

        builder
            .HasMany(u => u.Reactions)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // TODO 8 Задати логічну та достатню довжину полів, використовуючи Fluent API
        builder.Property(u => u.Username)
            .HasMaxLength(64);

        // TODO 7 Забезпечення унікальних ключів
        builder.HasIndex(u => u.Username)
            .IsUnique();

        // TODO 8 Задати логічну та достатню довжину полів, використовуючи Fluent API
        builder.Property(u => u.Email)
            .HasMaxLength(64);

        // TODO 7 Забезпечення унікальних ключів
        builder.HasIndex(u => u.Email)
            .IsUnique();

        // TODO 8 Задати логічну та достатню довжину полів, використовуючи Fluent API
        builder.Property(u => u.Password)
            .HasMaxLength(64);
    }
}