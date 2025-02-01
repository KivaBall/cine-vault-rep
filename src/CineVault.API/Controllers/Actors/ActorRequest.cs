namespace CineVault.API.Controllers.Actors;

public sealed class ActorRequest
{
    public required string FullName { get; init; }
    public required DateOnly BirthDate { get; init; }
    public required string Biography { get; init; }
}