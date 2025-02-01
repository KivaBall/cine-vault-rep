namespace CineVault.API.Controllers.Reviews;

public sealed class ReviewResponse
{
    public required int Id { get; init; }
    public required int MovieId { get; init; }
    public required string MovieTitle { get; init; }
    public required int UserId { get; init; }
    public required string Username { get; init; }
    public required int Rating { get; init; }
    public required string? Comment { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required ICollection<ReactionResponse> Reactions { get; init; }
    public required int Likes { get; init; }
    public required int Dislikes { get; init; }
}