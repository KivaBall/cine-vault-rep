namespace CineVault.API.Entities;

// TODO a) вставка нових даних, оновлення по фільмам в окрему таблицю/сутність бази даних
public sealed class MovieStat
{
    public MovieStat(int movieId, double averageRating, int reviewCount, bool movieWasDeleted)
    {
        MovieId = movieId;
        AverageRating = averageRating;
        ReviewCount = reviewCount;
        MovieWasDeleted = movieWasDeleted;
    }

    public int MovieId { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public bool MovieWasDeleted { get; set; }
    public DateTime LastChangedAt { get; set; }

    public Movie Movie { get; set; } // NavProperty
}