namespace CineVault.API.Entities;

public sealed class Movie : BaseEntity
{
    public Movie(string title, string? description, DateOnly? releaseDate, string? genre,
        string? director)
    {
        Title = title;
        Description = description;
        ReleaseDate = releaseDate;
        Genre = genre;
        Director = director;
    }

    public string Title { get; set; }
    public string? Description { get; set; }
    public string? Genre { get; set; }
    public string? Director { get; set; }
    public DateOnly? ReleaseDate { get; set; }

    public ICollection<Review> Reviews { get; } = []; // NavProperty
}