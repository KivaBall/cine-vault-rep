using CineVault.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineVault.API.Configuration;

public sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        // TODO 4 Зробити три зміни на власний розсуд в структурі бази даних та створити міграцію
        builder.Property(m => m.Id)
            .HasColumnName("ReviewId");

        // TODO 10 Налаштувати фільтри на рівні DbContext, щоб виключати видалені записи з запитів
        builder.HasQueryFilter(r => !r.IsDeleted);

        // TODO 8 Задати логічну та достатню довжину полів, використовуючи Fluent API
        builder.Property(r => r.Comment)
            .HasMaxLength(512);
    }
}