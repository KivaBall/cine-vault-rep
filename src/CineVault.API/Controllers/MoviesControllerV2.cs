using Asp.Versioning;
using CineVault.API.Abstractions.Controllers;
using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Entities;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineVault.API.Controllers;

public sealed partial class MoviesController
{
    [HttpPost("all")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<List<MovieResponse>>>> GetMoviesV2(
        BaseRequest<GetMoviesRequest> request)
    {
        logger.Information("Serilog | Getting movies...");

        var movies = await dbContext.Movies
            .AsNoTracking()
            .Where(m =>
                request.Data.Title == null
                || m.Title == request.Data.Title)
            .Where(m =>
                request.Data.Genre == null
                || m.Genre == request.Data.Genre)
            .Where(m =>
                request.Data.Director == null
                || m.Director == request.Data.Director)
            .Where(m =>
                request.Data.MinReleaseDate == null
                || m.ReleaseDate >= request.Data.MinReleaseDate)
            .Where(m =>
                request.Data.MaxReleaseDate == null
                || m.ReleaseDate <= request.Data.MaxReleaseDate)
            .Where(m =>
                request.Data.MinAvgRating == null
                || (m.Reviews.Count != 0 ? m.Reviews.Average(r => r.Rating) : 0) >=
                request.Data.MinAvgRating)
            .Where(m =>
                request.Data.MaxAvgRating == null
                || (m.Reviews.Count != 0 ? m.Reviews.Average(r => r.Rating) : 0) <=
                request.Data.MaxAvgRating)
            .Skip((request.Data.MoviesPerPage ?? 10) * ((request.Data.Page ?? 1) - 1))
            .Take(request.Data.MoviesPerPage ?? 10)
            .ProjectToType<MovieResponse>()
            .ToListAsync();

        return Ok(BaseResponse.Ok(movies, "All movies retrieved successfully"));
    }

    [HttpPost("search-movies")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<ICollection<MovieResponse>>>> SearchMoviesV2(
        BaseRequest<SearchMoviesRequest> request)
    {
        logger.Information("Serilog | Getting movies...");

        var searchText = request.Data.SearchText?.ToLower();

        var movies = await dbContext.Movies
            .AsNoTracking()
            .Where(m =>
                request.Data.SearchText == null
                || m.Title.ToLower().Contains(searchText!)
                || (m.Description != null && m.Description.ToLower().Contains(searchText!))
                || (m.Director != null && m.Director.ToLower().Contains(searchText!)))
            .Where(m =>
                request.Data.Genre == null
                || m.Genre == request.Data.Genre)
            .Where(m =>
                request.Data.MinAvgRating == null
                || (m.Reviews.Count != 0 ? m.Reviews.Average(r => r.Rating) : 0) >=
                request.Data.MinAvgRating)
            .Where(m =>
                request.Data.MinReleaseDate == null
                || m.ReleaseDate >= request.Data.MinReleaseDate)
            .Where(m =>
                request.Data.MaxReleaseDate == null
                || m.ReleaseDate <= request.Data.MaxReleaseDate)
            .ProjectToType<MovieResponse>()
            .ToListAsync();

        return Ok(BaseResponse.Ok(movies, "All movies retrieved successfully"));
    }

    [HttpPost("{id:int}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<MovieResponse>>> GetMovieByIdV2(
        BaseRequest request, int id)
    {
        logger.Information("Serilog | Getting movie with ID {Id}...", id);

        var movie = await dbContext.Movies
            .AsNoTracking()
            .ProjectToType<MovieResponse>()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movie == null)
        {
            logger.Warning("Serilog | Movie with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("Movie by ID was not found"));
        }

        return Ok(BaseResponse.Ok(movie, "Movie by ID retrieved successfully"));
    }

    [HttpPost("{id:int}/movie-details")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<MovieDetails>>> GetMovieDetailsV2(
        BaseRequest request, int id)
    {
        logger.Information("Serilog | Getting movie with ID {Id}...", id);

        var movie = await dbContext.Movies
            .AsNoTracking()
            .ProjectToType<MovieDetails>()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movie == null)
        {
            logger.Warning("Serilog | Movie with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("Movie by ID was not found"));
        }

        return Ok(BaseResponse.Ok(movie, "Movie details by ID retrieved successfully"));
    }

    [HttpPost("movie-stats")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<ICollection<MovieStat>>>> GetMovieStats(
        BaseRequest request)
    {
        var movieStats = await dbContext.MovieStats.ToListAsync();

        return Ok(BaseResponse.Ok(movieStats, "Movie stats retrieved successfully"));
    }

    [HttpPost]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<int>>> CreateMovieV2(
        BaseRequest<MovieRequest> request)
    {
        var titleExists = await dbContext.Movies
            .AnyAsync(m => m.Title == request.Data.Title);

        if (titleExists)
        {
            logger.Warning("Serilog | Movie with Title '{Title}' already exists",
                request.Data.Title);

            return BadRequest(BaseResponse.BadRequest("Movie title is already in use"));
        }

        var movie = mapper.Map<Movie>(request.Data);

        dbContext.Movies.Add(movie);

        logger.Information("Serilog | Adding movie...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Created(movie.Id, "Movie was created successfully"));
    }

    [HttpPost("several")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<ICollection<int>>>> CreateMoviesV2(
        BaseRequest<ICollection<MovieRequest>> request)
    {
        var requestedTitles = request.Data.Select(m => m.Title).ToList();

        var existingTitles = await dbContext.Movies
            .AsNoTracking()
            .Where(m => requestedTitles.Contains(m.Title))
            .Select(m => m.Title)
            .ToListAsync();

        var duplicateTitles = requestedTitles.Intersect(existingTitles).ToList();

        if (duplicateTitles.Count != 0)
        {
            logger.Warning("Serilog | Some movie titles already exist: {Titles}",
                string.Join(", ", duplicateTitles));

            return BadRequest(BaseResponse.BadRequest(
                $"Some movie titles are already in use: {string.Join(", ", duplicateTitles)}"));
        }

        var movies = request.Data
            .Select(mapper.Map<Movie>)
            .ToList();

        dbContext.Movies.AddRange(movies);

        logger.Information("Serilog | Adding movies...");

        await dbContext.SaveChangesAsync();

        var ids = movies
            .Select(m => m.Id)
            .ToList();

        return Ok(BaseResponse.Created(ids, "Movies were created successfully"));
    }

    [HttpPut("{id:int}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> UpdateMovieV2(int id,
        BaseRequest<MovieRequest> request)
    {
        logger.Information("Serilog | Getting movie with ID {Id}...", id);

        var movie = await dbContext.Movies.FindAsync(id);

        if (movie == null)
        {
            logger.Warning("Serilog | Movie with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("Movie by ID was not found"));
        }

        var titleExists = await dbContext.Movies
            .AnyAsync(m => m.Title == request.Data.Title && m.Id != id);

        if (titleExists)
        {
            logger.Warning("Serilog | Movie with Title '{Title}' already exists",
                request.Data.Title);
            return BadRequest(BaseResponse.BadRequest("Movie title is already in use"));
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

    [HttpDelete("{id:int}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> DeleteMovieV2(BaseRequest request, int id)
    {
        logger.Information("Serilog | Getting movie with ID {Id}...", id);

        var movie = await dbContext.Movies.FindAsync(id);

        if (movie == null)
        {
            logger.Warning("Serilog | Movie with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("Movie by ID was not found"));
        }

        movie.IsDeleted = true;

        dbContext.Movies.Update(movie);

        logger.Information("Serilog | Deleting movie...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Ok("Movie by ID was deleted successfully"));
    }

    [HttpDelete]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<string>>> DeleteMoviesV2(
        BaseRequest<ICollection<int>> request)
    {
        logger.Information("Serilog | Getting movies with specified IDs {Ids}...", request.Data);

        var data = await dbContext.Movies
            .Where(m => request.Data.Contains(m.Id))
            .Select(m => new
            {
                Movie = m,
                HasReview = m.Reviews.Any()
            })
            .ToListAsync();

        var deletedIds = new List<int>();
        var undeletedIds = new List<int>();

        foreach (var unit in data)
        {
            if (unit.HasReview)
            {
                logger.Warning(
                    "Serilog | Movie with ID {Id} cannot be deleted due to possession reviews",
                    unit.Movie.Id);

                undeletedIds.Add(unit.Movie.Id);
            }
            else
            {
                unit.Movie.IsDeleted = true;

                dbContext.Movies.Update(unit.Movie);

                deletedIds.Add(unit.Movie.Id);
            }
        }

        logger.Information("Serilog | Deleting movies...");

        await dbContext.SaveChangesAsync();

        var response = BaseResponse.Ok(deletedIds,
            "Movie by specified IDs in Data were deleted successfully. " +
            "The exception are movies IDs that have reviews were specified in Meta");

        response.Meta.Add("undeletedIds", undeletedIds);

        return Ok(response);
    }
}