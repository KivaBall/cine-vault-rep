namespace CineVault.API.Controllers.Reviews;

public sealed class ReviewRequest
{
    public required int MovieId { get; init; }
    public required int UserId { get; init; }
    public required int Rating { get; init; }
    public required string? Comment { get; init; }
}