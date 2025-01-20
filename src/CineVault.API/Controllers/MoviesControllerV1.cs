namespace CineVault.API.Controllers;

public sealed partial class MoviesController(CineVaultDbContext dbContext, ILogger logger) : BaseController
{
    [HttpGet]
    [MapToApiVersion(1)]
    public async Task<ActionResult<List<MovieResponse>>> GetMoviesV1()
    {
        logger.Information("Serilog | Getting movies...");

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
    [MapToApiVersion(1)]
    public async Task<ActionResult<MovieResponse>> GetMovieByIdV1(int id)
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
    [MapToApiVersion(1)]
    public async Task<ActionResult> CreateMovieV1(MovieRequest request)
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

        logger.Information("Serilog | Adding movie...");

        await dbContext.SaveChangesAsync();

        return Created();
    }

    [HttpPut("{id}")]
    [MapToApiVersion(1)]
    public async Task<ActionResult> UpdateMovieV1(int id, MovieRequest request)
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

        logger.Information("Serilog | Updating movie...");

        await dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{id}")]
    [MapToApiVersion(1)]
    public async Task<ActionResult> DeleteMovieV1(int id)
    {
        logger.Information("Serilog | Getting movie with ID {Id}...", id);

        var movie = await dbContext.Movies.FindAsync(id);

        if (movie is null)
        {
            logger.Warning("Serilog | Movie with ID {Id} not found", id);

            return NotFound();
        }

        dbContext.Movies.Remove(movie);

        logger.Information("Serilog | Deleting movie...");

        await dbContext.SaveChangesAsync();

        return Ok();
    }
}