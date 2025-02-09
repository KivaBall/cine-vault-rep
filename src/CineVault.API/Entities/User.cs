using CineVault.API.Abstractions.Entities;

namespace CineVault.API.Entities;

public sealed class User : BaseEntity
{
    public User(string username, string email, string password)
    {
        Username = username;
        Email = email;
        Password = password;
        CreatedAt = DateTime.UtcNow;
    }

    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Review> Reviews { get; } = []; // NavProperty
    public ICollection<Reaction> Reactions { get; } = []; // NavProperty
}