namespace CineVault.API.Controllers.Requests;

public sealed class ReactionRequest
{
    public required bool IsLike { get; init; }
    public required int ReviewId { get; init; }
    public required int UserId { get; init; }
}