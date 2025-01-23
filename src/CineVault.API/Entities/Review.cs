﻿namespace CineVault.API.Entities;

public sealed class Review : BaseEntity
{
    public Review(string? comment, int rating, int movieId, int userId)
    {
        Comment = comment;
        Rating = rating;
        MovieId = movieId;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }

    // TODO 4 Можливість ставити відгук без коментаря
    public string? Comment { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public int MovieId { get; set; }
    public int UserId { get; set; }

    // TODO 4 Зв'язати коментарі з користувачами та відгуками
    public Movie Movie { get; set; } // NavProperty
    public User User { get; set; } // NavProperty
    public ICollection<Reaction> Reactions { get; } = []; // NavProperty
}