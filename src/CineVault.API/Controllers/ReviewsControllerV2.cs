using System.Text.Json;
using Asp.Versioning;
using CineVault.API.Abstractions.Controllers;
using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Entities;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace CineVault.API.Controllers;

public sealed partial class ReviewsController
{
    private static readonly Func<CineVaultDbContext, int, int, Task<ReviewCheckResult?>>
        GetReviewCheck = EF.CompileAsyncQuery(
            (CineVaultDbContext context, int movieId, int userId) =>
                context.Movies
                    .AsNoTracking()
                    .Where(m => m.Id == movieId)
                    .Select(m => new ReviewCheckResult(
                        context.Users
                            .Any(u => u.Id == userId),
                        context.Reviews
                            .Any(r => r.MovieId == movieId && r.UserId == userId)
                    ))
                    .FirstOrDefault());

    [HttpPost("all")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<List<ReviewResponse>>>> GetReviewsV2(
        BaseRequest request)
    {
        logger.Information("Serilog | Getting reviews...");

        var reviews = await dbContext.Reviews
            .AsNoTracking()
            .ProjectToType<ReviewResponse>()
            .ToListAsync();

        return Ok(BaseResponse.Ok(reviews, "Reviews retrieved successfully"));
    }

    [HttpPost("{id:int}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<ReviewResponse>>> GetReviewByIdV2(
        BaseRequest request, int id)
    {
        logger.Information("Serilog | Getting review with ID {Id}...", id);

        var review = await dbContext.Reviews
            .AsNoTracking()
            .ProjectToType<ReviewResponse>()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (review == null)
        {
            logger.Warning("Serilog | Review with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("Review by ID was not found"));
        }

        return Ok(BaseResponse.Ok(review, "Review by ID retrieved successfully"));
    }

    // TODO b) додати в існуючий метод, який повертає огляд на фільм або додати новий, якщо відсутній, який кешуватиме список оглядів (рев'ю) для конкретного фільму у Distributed Cache (SQL Server)
    [HttpPost("by-movie/{movieId:int}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<ICollection<ReviewResponse>>>>
        GetReviewsByMovieIdV2(BaseRequest request, int movieId)
    {
        var cacheKey = $"reviews_{movieId}";

        logger.Information("Serilog | Getting reviews with movie ID {Id} from distributed cache...",
            movieId);

        var cachedReviewsJson = await distributedCache.GetStringAsync(cacheKey);

        // TODO b) якщо дані про огляди доступні у Distributed Cache, повертати їх із кешу (використовуючи ключ, який включає ID фільму, наприклад, reviews_{movieId}). 
        if (cachedReviewsJson != null)
        {
            var deserializedReviews = JsonSerializer.Deserialize<ICollection<ReviewResponse>>(
                cachedReviewsJson);

            return Ok(BaseResponse.Ok(deserializedReviews,
                "Reviews by movie ID retrieved from distributed cache successfully"));
        }

        logger.Information("Serilog | Getting reviews with movie ID {Id} from database...",
            movieId);

        // TODO b) якщо даних немає, отримувати їх із бази даних, серіалізувати у формат JSON, зберегти у кеші з часом життя 1 хвилина, а потім повертати користувачеві
        var reviews = await dbContext.Movies
            .AsNoTracking()
            .Where(m => m.Id == movieId)
            .Select(m => m.Reviews)
            .ProjectToType<ICollection<ReviewResponse>>()
            .FirstOrDefaultAsync();

        if (reviews == null)
        {
            logger.Warning("Serilog | Reviews with movie ID {Id} not found", movieId);

            return NotFound(BaseResponse.NotFound("Reviews by movie ID were not found"));
        }

        var serializedReviews = JsonSerializer.Serialize(reviews);

        logger.Information("Serilog | Caching reviews with movie ID {Id} in distributed cache...",
            movieId);

        await distributedCache.SetStringAsync(cacheKey, serializedReviews,
            new DistributedCacheEntryOptions
            {
                // TODO b) кеш має оновлюватися кожну 1 хвилину
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            });

        return Ok(BaseResponse.Ok(reviews,
            "Reviews by movie ID retrieved from database successfully"));
    }

    [HttpPost]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<int>>> CreateReviewV2(
        BaseRequest<ReviewRequest> request)
    {
        if (request.Data.Rating is < 1 or > 10)
        {
            logger.Warning("Serilog | Review has rating out of range - {Rating}",
                request.Data.Rating);

            return BadRequest(BaseResponse.BadRequest("Review rating is not in correct span"));
        }

        var data = await GetReviewCheck(dbContext, request.Data.MovieId, request.Data.UserId);

        if (data == null)
        {
            logger.Warning("Serilog | Specified movie ID cannot be found");

            return BadRequest(BaseResponse.BadRequest("Specified movie ID cannot be found"));
        }

        if (!data.UserExists)
        {
            logger.Warning("Serilog | Specified user ID cannot be found");

            return BadRequest(BaseResponse.BadRequest("Specified user ID cannot be found"));
        }

        var review = mapper.Map<Review>(request.Data);

        if (data.ReviewExists)
        {
            dbContext.Reviews.Update(review);

            logger.Information("Serilog | Updating review...");

            await dbContext.SaveChangesAsync();

            return Ok(BaseResponse.Ok("Review was updated successfully"));
        }

        dbContext.Reviews.Add(review);

        logger.Information("Serilog | Creating review...");

        await dbContext.SaveChangesAsync();

        logger.Information("Serilog | Deleting cache for reviews with movie ID {Id}...",
            request.Data.MovieId);

        // TODO b) додати логіку, яка видаляє кеш для оглядів, якщо додається/видаляється новий огляд до фільму
        await distributedCache.RemoveAsync($"reviews_{request.Data.MovieId}");

        return Ok(BaseResponse.Created(review.Id, "Review was created successfully"));
    }

    [HttpPut("{id:int}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> UpdateReviewV2(int id,
        BaseRequest<ReviewRequest> request)
    {
        if (request.Data.Rating is < 1 or > 10)
        {
            logger.Warning("Serilog | Review with ID {Id} has rating out of range - {Rating}", id,
                request.Data.Rating);

            return BadRequest(BaseResponse.BadRequest("Review rating is not in correct span"));
        }

        logger.Information("Serilog | Getting review with ID {Id}...", id);

        var review = await dbContext.Reviews.FindAsync(id);

        if (review == null)
        {
            logger.Warning("Serilog | Review with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("Review by ID was not found"));
        }

        review.MovieId = request.Data.MovieId;
        review.UserId = request.Data.UserId;
        review.Rating = request.Data.Rating;
        review.Comment = request.Data.Comment;

        logger.Information("Serilog | Updating review...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Ok("Review by ID was updated successfully"));
    }

    [HttpDelete("{id:int}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> DeleteReviewV2(BaseRequest request, int id)
    {
        logger.Information("Serilog | Getting review with ID {Id}...", id);

        var review = await dbContext.Reviews.FindAsync(id);

        if (review == null)
        {
            logger.Warning("Serilog | Review with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("Review by ID was not found"));
        }

        review.IsDeleted = true;

        dbContext.Reviews.Update(review);

        logger.Information("Serilog | Deleting review...");

        await dbContext.SaveChangesAsync();

        logger.Information("Serilog | Deleting cache for reviews with movie ID {Id}...",
            review.MovieId);

        // TODO b) додати логіку, яка видаляє кеш для оглядів, якщо додається/видаляється новий огляд до фільму
        await distributedCache.RemoveAsync($"reviews_{review.MovieId}");

        return Ok(BaseResponse.Ok("Review by ID was deleted successfully"));
    }

    private record ReviewCheckResult(bool UserExists, bool ReviewExists);
}