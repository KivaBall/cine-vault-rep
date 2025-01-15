namespace CineVault.API.Controllers;

public sealed class MoviesController(
    CineVaultDbContext dbContext,
    IMapper mapper)
    : BaseController
{
    [HttpGet]
    public async Task<ActionResult<List<MovieResponse>>> GetMovies()
    {
        var movies = await dbContext.Movies
            .Include(m => m.Reviews)
            .Select(m => mapper.Map<MovieResponse>(m))
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

        var response = mapper.Map<MovieResponse>(movie);

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult> CreateMovie(MovieRequest request)
    {
        var movie = mapper.Map<Movie>(request);

        dbContext.Movies.Add(movie);

        await dbContext.SaveChangesAsync();

        return Created();
    }

    [HttpPut("{id}")]
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