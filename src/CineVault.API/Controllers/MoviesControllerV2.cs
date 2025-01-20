namespace CineVault.API.Controllers;

public sealed partial class MoviesController
{
    [HttpGet]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<List<MovieResponse>>>> GetMoviesV2()
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

        return Ok(BaseResponse.Ok(movies, "All movies retrieved successfully"));
    }

    [HttpGet("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<MovieResponse>>> GetMovieByIdV2(int id)
    {
        logger.Information("Serilog | Getting movie with ID {Id}...", id);

        var movie = await dbContext.Movies
            .Include(m => m.Reviews)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movie is null)
        {
            logger.Warning("Serilog | Movie with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("Movie by ID was not found"));
        }

        var movieResponse = new MovieResponse
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

        return Ok(BaseResponse.Ok(movieResponse, "Movie by ID retrieved successfully"));
    }

    [HttpPost]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> CreateMovieV2(BaseRequest<MovieRequest> request)
    {
        var movie = new Movie
        {
            Title = request.Data.Title,
            Description = request.Data.Description,
            ReleaseDate = request.Data.ReleaseDate,
            Genre = request.Data.Genre,
            Director = request.Data.Director
        };

        dbContext.Movies.Add(movie);

        logger.Information("Serilog | Adding movie...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Created("Movie was created successfully"));
    }

    [HttpPut("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> UpdateMovieV2(int id,
        BaseRequest<MovieRequest> request)
    {
        logger.Information("Serilog | Getting movie with ID {Id}...", id);

        var movie = await dbContext.Movies.FindAsync(id);

        if (movie is null)
        {
            logger.Warning("Serilog | Movie with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("Movie by ID was not found"));
        }

        movie.Title = request.Data.Title;
        movie.Description = request.Data.Description;
        movie.ReleaseDate = request.Data.ReleaseDate;
        movie.Genre = request.Data.Genre;
        movie.Director = request.Data.Director;

        logger.Information("Serilog | Updating movie...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Ok("Movie by ID was updated successfully"));
    }

    [HttpDelete("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> DeleteMovieV2(int id)
    {
        logger.Information("Serilog | Getting movie with ID {Id}...", id);

        var movie = await dbContext.Movies.FindAsync(id);

        if (movie is null)
        {
            logger.Warning("Serilog | Movie with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("Movie by ID was not found"));
        }

        dbContext.Movies.Remove(movie);

        logger.Information("Serilog | Deleting movie...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Ok("Movie by ID was deleted successfully"));
    }
}