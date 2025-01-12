﻿namespace CineVault.API.Controllers;

public sealed class ReviewsController(CineVaultDbContext dbContext) : BaseController
{
    [HttpGet]
    public async Task<ActionResult<List<ReviewResponse>>> GetReviews()
    {
        var reviews = await dbContext.Reviews
            .Include(r => r.Movie)
            .Include(r => r.User)
            .Select(r => new ReviewResponse
            {
                Id = r.Id,
                MovieId = r.MovieId,
                MovieTitle = r.Movie!.Title,
                UserId = r.UserId,
                Username = r.User!.Username,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        return Ok(reviews);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewResponse>> GetReviewById(int id)
    {
        var review = await dbContext.Reviews
            .Include(r => r.Movie)
            .Include(r => r.User)
            .FirstOrDefaultAsync(review => review.Id == id);

        if (review is null)
        {
            return NotFound();
        }

        var response = new ReviewResponse
        {
            Id = review.Id,
            MovieId = review.MovieId,
            MovieTitle = review.Movie!.Title,
            UserId = review.UserId,
            Username = review.User!.Username,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult> CreateReview(ReviewRequest request)
    {
        var review = new Review
        {
            MovieId = request.MovieId,
            UserId = request.UserId,
            Rating = request.Rating,
            Comment = request.Comment
        };

        dbContext.Reviews.Add(review);
        await dbContext.SaveChangesAsync();

        return Created();
    }

    [HttpPut("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult> UpdateReview(int id, ReviewRequest request)
    {
        var review = await dbContext.Reviews.FindAsync(id);

        if (review is null)
        {
            return NotFound();
        }

        review.MovieId = request.MovieId;
        review.UserId = request.UserId;
        review.Rating = request.Rating;
        review.Comment = request.Comment;

        await dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteReview(int id)
    {
        var review = await dbContext.Reviews.FindAsync(id);

        if (review is null)
        {
            return NotFound();
        }

        dbContext.Reviews.Remove(review);
        await dbContext.SaveChangesAsync();

        return Ok();
    }
}