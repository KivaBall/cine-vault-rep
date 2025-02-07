using Asp.Versioning;
using CineVault.API.Abstractions.Controllers;
using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Entities;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace CineVault.API.Controllers;

public sealed partial class ReviewsController(
    CineVaultDbContext dbContext,
    ILogger logger,
    IMapper mapper)
    : BaseController
{
    [HttpGet]
    [MapToApiVersion(1)]
    public async Task<ActionResult<List<ReviewResponse>>> GetReviewsV1()
    {
        logger.Information("Serilog | Getting reviews...");

        var reviews = await dbContext.Reviews
            .Include(r => r.Movie)
            .Include(r => r.User)
            .Select(r => mapper.Map<ReviewResponse>(r))
            .ToListAsync();

        return Ok(reviews);
    }

    [HttpGet("{id:int}")]
    [MapToApiVersion(1)]
    public async Task<ActionResult<ReviewResponse>> GetReviewByIdV1(int id)
    {
        logger.Information("Serilog | Getting review with ID {Id}...", id);

        var review = await dbContext.Reviews
            .Include(r => r.Movie)
            .Include(r => r.User)
            .FirstOrDefaultAsync(review => review.Id == id);

        if (review is null)
        {
            logger.Warning("Serilog | Review with ID {Id} not found", id);

            return NotFound();
        }

        var response = mapper.Map<ReviewResponse>(review);

        return Ok(response);
    }

    [HttpPost]
    [MapToApiVersion(1)]
    public async Task<ActionResult> CreateReviewV1(ReviewRequest request)
    {
        var review = mapper.Map<Review>(request);

        dbContext.Reviews.Add(review);

        logger.Information("Serilog | Creating review...");

        await dbContext.SaveChangesAsync();

        return Created();
    }

    [HttpPut("{id:int}")]
    [MapToApiVersion(1)]
    public async Task<ActionResult> UpdateReviewV1(int id, ReviewRequest request)
    {
        logger.Information("Serilog | Getting review with ID {Id}...", id);

        var review = await dbContext.Reviews.FindAsync(id);

        if (review is null)
        {
            logger.Warning("Serilog | Review with ID {Id} not found", id);

            return NotFound();
        }

        review.MovieId = request.MovieId;
        review.UserId = request.UserId;
        review.Rating = request.Rating;
        review.Comment = request.Comment;

        logger.Information("Serilog | Updating review...");

        await dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{id:int}")]
    [MapToApiVersion(1)]
    public async Task<ActionResult> DeleteReviewV1(int id)
    {
        logger.Information("Serilog | Getting review with ID {Id}...", id);

        var review = await dbContext.Reviews.FindAsync(id);

        if (review is null)
        {
            logger.Warning("Serilog | Review with ID {Id} not found", id);

            return NotFound();
        }

        dbContext.Reviews.Remove(review);

        logger.Information("Serilog | Deleting review...");

        await dbContext.SaveChangesAsync();

        return Ok();
    }
}