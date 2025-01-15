namespace CineVault.API.Controllers;

public sealed class ReviewsController(
    CineVaultDbContext dbContext,
    IMapper mapper)
    : BaseController
{
    [HttpGet]
    public async Task<ActionResult<List<ReviewResponse>>> GetReviews()
    {
        var reviews = await dbContext.Reviews
            .Include(r => r.Movie)
            .Include(r => r.User)
            .Select(r => mapper.Map<ReviewResponse>(r))
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

        var response = mapper.Map<ReviewResponse>(review);

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult> CreateReview(ReviewRequest request)
    {
        var review = mapper.Map<Review>(request);

        dbContext.Reviews.Add(review);

        await dbContext.SaveChangesAsync();

        return Created();
    }

    [HttpPut("{id}")]
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