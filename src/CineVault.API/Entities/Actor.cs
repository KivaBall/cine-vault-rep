using CineVault.API.Abstractions.Entities;

namespace CineVault.API.Entities;

// TODO 5 Необхідно додати нову сутність Actor, яка буде представляти акторів у базі даних. Сутність повина мати поля FullName, BirthDate, Biography.
public sealed class Actor : BaseEntity
{
    public Actor(string fullName, DateOnly birthDate, string biography)
    {
        FullName = fullName;
        BirthDate = birthDate;
        Biography = biography;
    }

    public string FullName { get; set; }
    public DateOnly BirthDate { get; set; }
    public string Biography { get; set; }

    // TODO 5 Встановити зв’язок типу "багато до багатьох" між сутністю Movie (фільм) та новою сутністю Actor
    public ICollection<Movie> Movies { get; } = []; // NavProperty
}