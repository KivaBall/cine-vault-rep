namespace CineVault.API.Controllers.Reactions;

public sealed class ReactionRequest
{
    public required bool IsLike { get; init; }
    public required int ReviewId { get; init; }
    public required int UserId { get; init; }
}