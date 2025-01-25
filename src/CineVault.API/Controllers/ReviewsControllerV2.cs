namespace CineVault.API.Controllers;

public sealed partial class ReviewsController
{
    [HttpPost("all")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<List<ReviewResponse>>>> GetReviewsV2(
        BaseRequest request)
    {
        logger.Information("Serilog | Getting reviews...");

        var reviews = await dbContext.Reviews
            .Include(r => r.Movie)
            .Include(r => r.User)
            .Select(r => mapper.Map<ReviewResponse>(r))
            .ToListAsync();

        return Ok(BaseResponse.Ok(reviews, "Reviews retrieved successfully"));
    }

    [HttpPost("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<ReviewResponse>>> GetReviewByIdV2(
        BaseRequest request, int id)
    {
        logger.Information("Serilog | Getting review with ID {Id}...", id);

        var review = await dbContext.Reviews
            .Include(r => r.Movie)
            .Include(r => r.User)
            .FirstOrDefaultAsync(review => review.Id == id);

        if (review is null)
        {
            logger.Warning("Serilog | Review with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("Review by ID was not found"));
        }

        var response = mapper.Map<ReviewResponse>(review);

        return Ok(BaseResponse.Ok(response, "Review by ID retrieved successfully"));
    }

    [HttpPost]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> CreateReviewV2(BaseRequest<ReviewRequest> request)
    {
        var review = mapper.Map<Review>(request.Data);

        dbContext.Reviews.Add(review);

        logger.Information("Serilog | Creating review...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Created("Review was created successfully"));
    }

    [HttpPut("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> UpdateReviewV2(int id,
        BaseRequest<ReviewRequest> request)
    {
        logger.Information("Serilog | Getting review with ID {Id}...", id);

        var review = await dbContext.Reviews.FindAsync(id);

        if (review is null)
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

    [HttpDelete("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> DeleteReviewV2(BaseRequest request, int id)
    {
        logger.Information("Serilog | Getting review with ID {Id}...", id);

        var review = await dbContext.Reviews.FindAsync(id);

        if (review is null)
        {
            logger.Warning("Serilog | Review with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("Review by ID was not found"));
        }

        dbContext.Reviews.Remove(review);

        logger.Information("Serilog | Deleting review...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Ok("Review by ID was deleted successfully"));
    }
}