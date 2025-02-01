namespace CineVault.API.Configuration;

public sealed class ActorConfiguration : IEntityTypeConfiguration<Actor>
{
    public void Configure(EntityTypeBuilder<Actor> builder)
    {
        // TODO 4 Зробити три зміни на власний розсуд в структурі бази даних та створити міграцію
        builder.HasKey(x => x.Id)
            .HasName("ActorId");

        // TODO 10 Налаштувати фільтри на рівні DbContext, щоб виключати видалені записи з запитів
        builder.HasQueryFilter(a => !a.IsDeleted);

        // TODO 8 Задати логічну та достатню довжину полів, використовуючи Fluent API
        builder.Property(a => a.FullName)
            .HasMaxLength(64);

        // TODO 7 Забезпечення унікальних ключів
        builder.HasIndex(a => a.FullName)
            .IsUnique();

        // TODO 8 Задати логічну та достатню довжину полів, використовуючи Fluent API
        builder.Property(a => a.Biography)
            .HasMaxLength(512);
    }
}