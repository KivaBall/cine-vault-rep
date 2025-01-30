namespace CineVault.API.Entities;

// TODO 5 Підтримка лайків для відгуків-коментарів з оцінкою
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
    // TODO 5 Ставити відгук може лише зареєстрований користувач
    public User User { get; set; } // NavProperty
}