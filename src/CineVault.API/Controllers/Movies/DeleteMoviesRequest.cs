namespace CineVault.API.Controllers.Movies;

public sealed class DeleteMoviesRequest
{
    public ICollection<int> Ids { get; init; }
}