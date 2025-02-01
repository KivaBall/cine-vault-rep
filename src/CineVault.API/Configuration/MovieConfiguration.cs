namespace CineVault.API.Configuration;

public sealed class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        // TODO 4 Зробити три зміни на власний розсуд в структурі бази даних та створити міграцію
        builder.HasKey(x => x.Id)
            .HasName("MovieId");

        // TODO 10 Налаштувати фільтри на рівні DbContext, щоб виключати видалені записи з запитів
        builder.HasQueryFilter(m => !m.IsDeleted);

        // TODO 8 Задати логічну та достатню довжину полів, використовуючи Fluent API
        builder.Property(m => m.Title)
            .HasMaxLength(64);

        // TODO 7 Забезпечення унікальних ключів
        builder.HasIndex(m => m.Title)
            .IsUnique();

        // TODO 8 Задати логічну та достатню довжину полів, використовуючи Fluent API
        builder.Property(m => m.Description)
            .HasMaxLength(512);

        // TODO 8 Задати логічну та достатню довжину полів, використовуючи Fluent API
        builder.Property(m => m.Genre)
            .HasMaxLength(64);

        // TODO 8 Задати логічну та достатню довжину полів, використовуючи Fluent API
        builder.Property(m => m.Director)
            .HasMaxLength(64);
    }
}