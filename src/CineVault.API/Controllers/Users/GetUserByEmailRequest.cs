namespace CineVault.API.Controllers.Users;

public sealed class GetUserByEmailRequest
{
    public required string Email { get; init; }
}