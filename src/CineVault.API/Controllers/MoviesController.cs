namespace CineVault.API.Controllers;

public sealed class MoviesController(CineVaultDbContext dbContext, ILogger logger) : BaseController
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
        logger.Information("Serilog | Getting movie with ID {Id}...", id);

        var movie = await dbContext.Movies
            .Include(m => m.Reviews)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movie is null)
        {
            logger.Warning("Serilog | Movie with ID {Id} not found", id);

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

        dbContext.Movies.Add(movie);

        await dbContext.SaveChangesAsync();

        return Created();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateMovie(int id, MovieRequest request)
    {
        logger.Information("Serilog | Getting movie with ID {Id}...", id);

        var movie = await dbContext.Movies.FindAsync(id);

        if (movie is null)
        {
            logger.Warning("Serilog | Movie with ID {Id} not found", id);

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
        logger.Information("Serilog | Getting movie with ID {Id}...", id);

        var movie = await dbContext.Movies.FindAsync(id);

        if (movie is null)
        {
            logger.Warning("Serilog | Movie with ID {Id} not found", id);

            return NotFound();
        }

        dbContext.Movies.Remove(movie);

        await dbContext.SaveChangesAsync();

        return Ok();
    }
}