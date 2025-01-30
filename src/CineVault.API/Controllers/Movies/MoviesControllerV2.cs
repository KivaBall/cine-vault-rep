namespace CineVault.API.Controllers.Movies;

public sealed partial class MoviesController
{
    [HttpPost("all")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<List<MovieResponse>>>> GetMoviesV2(
        // TODO 3 Реалізувати складні запити, які дозволяють комбінувати кілька критеріїв
        BaseRequest<GetMoviesRequest> request)
    {
        logger.Information("Serilog | Getting movies...");

        var query = dbContext.Movies.AsQueryable();

        // TODO 3 Реалізувати пошук фільмів за жанром, назвою або режисером 
        if (request.Data.Title != null)
        {
            query = query.Where(m => m.Title == request.Data.Title);
        }

        if (request.Data.Genre != null)
        {
            query = query.Where(m => m.Genre == request.Data.Genre);
        }

        if (request.Data.Director != null)
        {
            query = query.Where(m => m.Director == request.Data.Director);
        }

        // TODO 3 Додати фільтрацію за роком випуску та середнім рейтингом
        if (request.Data.MinReleaseDate != null)
        {
            query = query.Where(m => m.ReleaseDate >= request.Data.MinReleaseDate);
        }

        if (request.Data.MaxReleaseDate != null)
        {
            query = query.Where(m => m.ReleaseDate <= request.Data.MaxReleaseDate);
        }

        if (request.Data.MinAvgRating != null)
        {
            query = query.Where(m => (m.Reviews.Count != 0
                ? m.Reviews.Average(r => r.Rating)
                : 0) >= request.Data.MinAvgRating);
        }

        if (request.Data.MaxAvgRating != null)
        {
            query = query.Where(m => (m.Reviews.Count != 0
                ? m.Reviews.Average(r => r.Rating)
                : 0) <= request.Data.MaxAvgRating);
        }

        var movies = await query
            .Include(m => m.Reviews)
            .ThenInclude(r => r.Reactions)
            .Include(m => m.Reviews)
            .ThenInclude(r => r.User)
            .Select(m => mapper.Map<MovieResponse>(m))
            .ToListAsync();

        return Ok(BaseResponse.Ok(movies, "All movies retrieved successfully"));
    }

    [HttpPost("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<MovieResponse>>> GetMovieByIdV2(
        BaseRequest request, int id)
    {
        logger.Information("Serilog | Getting movie with ID {Id}...", id);

        var movie = await dbContext.Movies
            .Include(m => m.Reviews)
            .ThenInclude(r => r.Reactions)
            .Include(m => m.Reviews)
            .ThenInclude(r => r.User)
            .Where(m => m.Id == id)
            .Select(m => mapper.Map<MovieResponse>(m))
            .FirstOrDefaultAsync();

        if (movie is null)
        {
            logger.Warning("Serilog | Movie with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("Movie by ID was not found"));
        }

        return Ok(BaseResponse.Ok(movie, "Movie by ID retrieved successfully"));
    }

    [HttpPost]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<int>>> CreateMovieV2(
        BaseRequest<MovieRequest> request)
    {
        var movie = mapper.Map<Movie>(request.Data);

        dbContext.Movies.Add(movie);

        logger.Information("Serilog | Adding movie...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Created(movie.Id, "Movie was created successfully"));
    }

    // TODO 1 Додати реалізацію масового завантаження фільмів
    [HttpPost("several")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<ICollection<int>>>> CreateMoviesV2(
        BaseRequest<ICollection<MovieRequest>> request)
    {
        var movies = request.Data.Select(mapper.Map<Movie>).ToList();

        dbContext.Movies.AddRange(movies);

        logger.Information("Serilog | Adding movies...");

        await dbContext.SaveChangesAsync();

        var ids = movies.Select(m => m.Id).ToList();

        return Ok(BaseResponse.Created(ids, "Movies were created successfully"));
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

    // TODO 7 Додати реалізацію для масового видалення за списком ID
    [HttpDelete]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<string>>> DeleteMoviesV2(
        BaseRequest<DeleteMoviesRequest> request)
    {
        logger.Information("Serilog | Getting movies with specified IDs {Ids}...",
            request.Data.Ids);

        var tuples = await dbContext.Movies
            .Where(m => request.Data.Ids.Contains(m.Id))
            .Select(m => new
            {
                Movie = m,
                HasReview = dbContext.Reviews.Any(r => r.MovieId == m.Id)
            })
            .ToListAsync();

        var deletedIds = new List<int>();
        var undeletedIds = new List<int>();

        // TODO 7 Додати перевірку, чи є фільми у відгуках, перед видаленням. Якщо є, то не видаляти такий, а виводити попередження, а інші фільми з масиву видалити
        foreach (var tuple in tuples)
        {
            if (tuple.HasReview)
            {
                logger.Warning(
                    "Serilog | Movie with ID {Id} cannot be deleted due to possession reviews",
                    tuple.Movie.Id);

                undeletedIds.Add(tuple.Movie.Id);
            }
            else
            {
                dbContext.Movies.Remove(tuple.Movie);

                deletedIds.Add(tuple.Movie.Id);
            }
        }

        logger.Information("Serilog | Deleting movies...");

        await dbContext.SaveChangesAsync();

        var response = BaseResponse.Ok(deletedIds,
            "Movie by specified IDs in Data were deleted successfully. The exception are movies IDs that have reviews were specified in Meta");

        response.Meta.Add("undeletedIds", undeletedIds);

        return Ok(response);
    }
}