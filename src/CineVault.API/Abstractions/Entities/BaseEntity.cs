namespace CineVault.API.Abstractions.Entities;

// TODO 10 Налаштувати підтримку "м'якого видалення" (Soft Delete) для сутностей.
// Додати поле IsDeleted (property) до кожної сутності
public abstract class BaseEntity
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; }
}