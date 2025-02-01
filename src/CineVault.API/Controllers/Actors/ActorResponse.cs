namespace CineVault.API.Controllers.Actors;

public sealed class ActorResponse
{
    public required int Id { get; init; }
    public required string FullName { get; init; }
    public required DateOnly BirthDate { get; init; }
    public required string Biography { get; init; }
}