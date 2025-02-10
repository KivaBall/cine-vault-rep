using CineVault.API.Abstractions.Entities;

namespace CineVault.API.Entities;

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

    public ICollection<Movie> Movies { get; } = []; // NavProperty
}