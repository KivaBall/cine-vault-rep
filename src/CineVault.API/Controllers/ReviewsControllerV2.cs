﻿using Asp.Versioning;
using CineVault.API.Abstractions.Controllers;
using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Entities;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineVault.API.Controllers;

public sealed partial class ReviewsController
{
    private static readonly Func<CineVaultDbContext, int, int, Task<ReviewCheckResult?>>
        GetReviewCheck = EF.CompileAsyncQuery(
            (CineVaultDbContext context, int movieId, int userId) =>
                context.Reviews
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

        // TODO 13 Визначити, де у вашому проєкті використовуються запити лише для читання даних, та додати AsNoTracking до них
        // TODO 13 Проаналізувати, чи не додаєте ви зайвих Include у запитах
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

        // TODO 13 Визначити, де у вашому проєкті використовуються запити лише для читання даних, та додати AsNoTracking до них
        // TODO 13 Проаналізувати, чи не додаєте ви зайвих Include у запитах
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

        // TODO 13 Визначити, де у вашому проєкті використовуються запити лише для читання даних, та додати AsNoTracking до них
        // TODO 13 Оптимізуйте місця в коді, де виникають кілька запитів на отримання даних, об'єднавши їх у один запит
        // TODO 13 Для часто виконуваних запитів створіть скомпільовані запити (CompileAsyncQuery)
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

        return Ok(BaseResponse.Ok("Review by ID was deleted successfully"));
    }

    private record ReviewCheckResult(bool UserExists, bool ReviewExists);
}