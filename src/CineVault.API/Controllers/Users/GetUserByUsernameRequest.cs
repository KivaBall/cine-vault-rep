namespace CineVault.API.Controllers.Users;

public sealed class GetUserByUsernameRequest
{
    public required string Username { get; init; }
}