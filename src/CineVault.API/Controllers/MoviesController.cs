namespace CineVault.API.Controllers;

public sealed class MoviesController(
    CineVaultDbContext dbContext,
    ILogger logger,
    IMapper mapper)
    : BaseController
{
    [HttpGet]
    public async Task<ActionResult<List<MovieResponse>>> GetMovies()
    {
        logger.Information("Serilog | Getting movies...");

        var movies = await dbContext.Movies
            .Include(m => m.Reviews)
            .Select(m => mapper.Map<MovieResponse>(m))
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

        var response = mapper.Map<MovieResponse>(movie);

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult> CreateMovie(MovieRequest request)
    {
        var movie = mapper.Map<Movie>(request);

        dbContext.Movies.Add(movie);

        logger.Information("Serilog | Adding movie...");

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

        logger.Information("Serilog | Updating movie...");

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

        logger.Information("Serilog | Deleting movie...");

        await dbContext.SaveChangesAsync();

        return Ok();
    }
}