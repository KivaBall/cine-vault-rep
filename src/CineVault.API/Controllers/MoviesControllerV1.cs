using Asp.Versioning;
using CineVault.API.Abstractions.Controllers;
using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Entities;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ILogger = Serilog.ILogger;

namespace CineVault.API.Controllers;

public sealed partial class MoviesController(
    CineVaultDbContext dbContext,
    ILogger logger,
    IMapper mapper,
    IMemoryCache memoryCache)
    : BaseController
{
    [HttpGet]
    [MapToApiVersion(1)]
    public async Task<ActionResult<List<MovieResponse>>> GetMoviesV1()
    {
        logger.Information("Serilog | Getting movies...");

        var movies = await dbContext.Movies
            .Include(m => m.Reviews)
            .Select(m => mapper.Map<MovieResponse>(m))
            .ToListAsync();

        return Ok(movies);
    }

    [HttpGet("{id:int}")]
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

        var response = mapper.Map<MovieResponse>(movie);

        return Ok(response);
    }

    [HttpPost]
    [MapToApiVersion(1)]
    public async Task<ActionResult> CreateMovieV1(MovieRequest request)
    {
        var movie = mapper.Map<Movie>(request);

        dbContext.Movies.Add(movie);

        logger.Information("Serilog | Adding movie...");

        await dbContext.SaveChangesAsync();

        return Created();
    }

    [HttpPut("{id:int}")]
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

    [HttpDelete("{id:int}")]
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