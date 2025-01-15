namespace CineVault.API.Controllers.Responses;

public sealed class MovieResponse
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required string? Description { get; set; }
    public required DateOnly? ReleaseDate { get; set; }
    public required string? Genre { get; set; }
    public required string? Director { get; set; }
    public required double AverageRating { get; set; }
    public required int ReviewCount { get; set; }
}