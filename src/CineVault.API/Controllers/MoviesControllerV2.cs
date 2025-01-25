namespace CineVault.API.Controllers;

public sealed partial class MoviesController
{
    [HttpPost("all")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<List<MovieResponse>>>> GetMoviesV2(
        BaseRequest request)
    {
        logger.Information("Serilog | Getting movies...");

        var movies = await dbContext.Movies
            .Include(m => m.Reviews)
            .Select(m => mapper.Map<MovieResponse>(m))
            .ToListAsync();

        return Ok(BaseResponse.Ok(movies, "All movies retrieved successfully"));
    }

    [HttpPost("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<MovieResponse>>> GetMovieByIdV2(BaseRequest request,
        int id)
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

        var response = mapper.Map<MovieResponse>(movie);

        return Ok(BaseResponse.Ok(response, "Movie by ID retrieved successfully"));
    }

    [HttpPost]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> CreateMovieV2(BaseRequest<MovieRequest> request)
    {
        var movie = mapper.Map<Movie>(request.Data);

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
    public async Task<ActionResult<BaseResponse>> DeleteMovieV2(BaseRequest request, int id)
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