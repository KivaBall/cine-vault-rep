namespace CineVault.API.Controllers.Reactions;

public sealed class GetReactionsByMovieRequest
{
    public required int MovieId { get; init; }
}