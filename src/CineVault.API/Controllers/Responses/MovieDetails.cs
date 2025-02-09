namespace CineVault.API.Controllers.Responses;

// TODO 9 Додати такі нові методи в API
public sealed class MovieDetails
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public required string? Description { get; init; }
    public required DateOnly? ReleaseDate { get; init; }
    public required string? Genre { get; init; }
    public required string? Director { get; init; }
    public required double AverageRating { get; init; }
    public required int ReviewCount { get; init; }
    public required ICollection<ReviewResponse> LastReviews { get; init; }
}