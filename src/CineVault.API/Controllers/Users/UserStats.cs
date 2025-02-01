namespace CineVault.API.Controllers.Users;

// TODO 9 Додати такі нові методи в API
public sealed class UserStats
{
    public required int Id { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required int ReviewCount { get; init; }
    public required double AverageRating { get; init; }
    public required ICollection<string> PopularUserGenres { get; init; }
    public required DateTime LastActivity { get; init; }
}