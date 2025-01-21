﻿namespace CineVault.API.Controllers.Users;

public sealed class UserResponse
{
    public required int Id { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required DateTime CreatedAt { get; init; }
}