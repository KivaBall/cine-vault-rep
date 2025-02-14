namespace CineVault.API.Controllers.Requests;

public sealed class SearchMoviesRequest
{
    public required string? SearchText { get; init; }
    public required string? Genre { get; init; }
    public required int? MinAvgRating { get; init; }
    public required DateOnly? MinReleaseDate { get; init; }
    public required DateOnly? MaxReleaseDate { get; init; }
}