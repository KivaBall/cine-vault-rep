namespace CineVault.API.Controllers.Reviews;

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
    public async Task<ActionResult<BaseResponse<int>>> CreateReviewV2(
        BaseRequest<ReviewRequest> request)
    {
        if (request.Data.Rating < 1 || request.Data.Rating > 10)
        {
            logger.Warning(
                "Serilog | Review has rating out of range - {Rating}",
                request.Data.Rating);

            return NotFound(BaseResponse.BadRequest("Review rating is not in correct span"));
        }

        var hypotheticReview = await dbContext.Reviews
            .Where(r => r.MovieId == request.Data.MovieId)
            .Where(r => r.UserId == request.Data.UserId)
            .FirstOrDefaultAsync();

        if (hypotheticReview is not null)
        {
            logger.Warning("Serilog | Review for such User and Movie IDs has been existed");

            return BadRequest(BaseResponse.BadRequest(
                "Review for such User and Movie IDs has been existed"));
        }

        var review = mapper.Map<Review>(request);

        dbContext.Reviews.Add(review);

        logger.Information("Serilog | Creating review...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Created(review.Id, "Review was created successfully"));
    }

    [HttpPut("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> UpdateReviewV2(int id,
        BaseRequest<ReviewRequest> request)
    {
        if (request.Data.Rating < 1 || request.Data.Rating > 10)
        {
            logger.Warning(
                "Serilog | Review with ID {Id} has rating out of range - {Rating}",
                id,
                request.Data.Rating);

            return NotFound(BaseResponse.BadRequest("Review rating is not in correct span"));
        }

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