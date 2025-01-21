namespace CineVault.API.Entities;

public sealed class Review : BaseEntity
{
    public Review(string? comment, int rating, int movieId, int userId)
    {
        Comment = comment;
        Rating = rating;
        MovieId = movieId;
        UserId = userId;
    }

    public string? Comment { get; set; }
    public int Rating { get; set; }
    public int MovieId { get; set; }
    public int UserId { get; set; }

    public Movie? Movie { get; } // NavProperty
    public User? User { get; } // NavProperty
    public ICollection<Reaction> Reactions { get; } = []; // NavProperty
}