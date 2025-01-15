namespace CineVault.API.Controllers.Requests;

public sealed class MovieRequest
{
    public required string Title { get; init; }
    public required string? Description { get; init; }
    public required DateOnly? ReleaseDate { get; init; }
    public required string? Genre { get; init; }
    public required string? Director { get; init; }
}