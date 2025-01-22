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
            .Include(r => r.Reactions)
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
            .Include(r => r.Reactions)
            .Where(r => r.Id == id)
            .Select(r => mapper.Map<ReviewResponse>(r))
            .FirstOrDefaultAsync();

        if (review is null)
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

        var movieExists = await dbContext.Movies.AnyAsync(m => m.Id == request.Data.MovieId);

        if (!movieExists)
        {
            logger.Warning("Serilog | Specified movie ID cannot be found");

            return BadRequest(BaseResponse.BadRequest("Specified movie ID cannot be found"));
        }

        var userExists = await dbContext.Users.AnyAsync(u => u.Id == request.Data.UserId);

        if (!userExists)
        {
            logger.Warning("Serilog | Specified user ID cannot be found");

            return BadRequest(BaseResponse.BadRequest("Specified user ID cannot be found"));
        }

        var reviewExists = await dbContext.Reviews
            .AnyAsync(r =>
                r.MovieId == request.Data.MovieId &&
                r.UserId == request.Data.UserId);

        if (reviewExists)
        {
            logger.Warning("Serilog | Review for such User and Movie IDs has been already created");

            return BadRequest(BaseResponse.BadRequest(
                "Review for such User and Movie IDs has been already created"));
        }

        var review = mapper.Map<Review>(request.Data);

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
        if (request.Data.Rating is < 1 or > 10)
        {
            logger.Warning(
                "Serilog | Review with ID {Id} has rating out of range - {Rating}",
                id, request.Data.Rating);

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