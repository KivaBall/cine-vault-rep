﻿namespace CineVault.API.Controllers;

public sealed class MoviesController(CineVaultDbContext dbContext) : BaseController
{
    [HttpGet]
    public async Task<ActionResult<List<MovieResponse>>> GetMovies()
    {
        var movies = await dbContext.Movies
            .Include(m => m.Reviews)
            .Select(m => new MovieResponse
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                ReleaseDate = m.ReleaseDate,
                Genre = m.Genre,
                Director = m.Director,
                AverageRating = m.Reviews.Count != 0
                    ? m.Reviews.Average(r => r.Rating)
                    : 0,
                ReviewCount = m.Reviews.Count
            })
            .ToListAsync();

        return Ok(movies);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MovieResponse>> GetMovieById(int id)
    {
        var movie = await dbContext.Movies
            .Include(m => m.Reviews)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movie is null)
        {
            return NotFound();
        }

        var response = new MovieResponse
        {
            Id = movie.Id,
            Title = movie.Title,
            Description = movie.Description,
            ReleaseDate = movie.ReleaseDate,
            Genre = movie.Genre,
            Director = movie.Director,
            AverageRating = movie.Reviews.Count != 0
                ? movie.Reviews.Average(r => r.Rating)
                : 0,
            ReviewCount = movie.Reviews.Count
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult> CreateMovie(MovieRequest request)
    {
        var movie = new Movie
        {
            Title = request.Title,
            Description = request.Description,
            ReleaseDate = request.ReleaseDate,
            Genre = request.Genre,
            Director = request.Director
        };

        await dbContext.Movies.AddAsync(movie);
        await dbContext.SaveChangesAsync();

        return Created();
    }

    [HttpPut("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult> UpdateMovie(int id, MovieRequest request)
    {
        var movie = await dbContext.Movies.FindAsync(id);

        if (movie is null)
        {
            return NotFound();
        }

        movie.Title = request.Title;
        movie.Description = request.Description;
        movie.ReleaseDate = request.ReleaseDate;
        movie.Genre = request.Genre;
        movie.Director = request.Director;

        await dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMovie(int id)
    {
        var movie = await dbContext.Movies.FindAsync(id);

        if (movie is null)
        {
            return NotFound();
        }

        dbContext.Movies.Remove(movie);
        await dbContext.SaveChangesAsync();

        return Ok();
    }
}