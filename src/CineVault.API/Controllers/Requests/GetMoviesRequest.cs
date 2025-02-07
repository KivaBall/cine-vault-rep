namespace CineVault.API.Controllers.Requests;

public sealed class GetMoviesRequest
{
    public required string? Title { get; init; }
    public required string? Genre { get; init; }
    public required string? Director { get; init; }
    public required DateOnly? MinReleaseDate { get; init; }
    public required DateOnly? MaxReleaseDate { get; init; }
    public required int? MinAvgRating { get; init; }
    public required int? MaxAvgRating { get; init; }
    public required int? MoviesPerPage { get; init; }
    public required int? Page { get; init; }
}