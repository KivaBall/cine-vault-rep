namespace CineVault.API.Controllers.Users;

public sealed class GetUsersRequest
{
    public required DateTime? MinCreatedDate { get; init; }
    public required DateTime? MaxCreatedDate { get; init; }
    public required bool? SortByAsc { get; init; }
    public required int? UsersPerPage { get; init; }
    public required int? Page { get; init; }
}