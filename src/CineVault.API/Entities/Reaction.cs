namespace CineVault.API.Entities;

public sealed class Reaction : BaseEntity
{
    public Reaction(bool isLike, int reviewId, int userId)
    {
        IsLike = isLike;
        ReviewId = reviewId;
        UserId = userId;
    }

    public bool IsLike { get; set; }
    public int ReviewId { get; set; }
    public int UserId { get; set; }

    public Review Review { get; set; } // NavProperty
    public User User { get; set; } // NavProperty
}