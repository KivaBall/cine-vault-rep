namespace CineVault.API.Controllers.Reactions;

public sealed class ReactionsController(
    CineVaultDbContext dbContext,
    ILogger logger,
    IMapper mapper)
    : BaseController
{
    [HttpPost]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<int>>> CreateReactionV2(
        BaseRequest<ReactionRequest> request)
    {
        var hypotheticReaction = await dbContext.Reactions
            .Where(r => r.ReviewId == request.Data.ReviewId)
            .Where(r => r.UserId == request.Data.UserId)
            .FirstOrDefaultAsync();

        if (hypotheticReaction is not null)
        {
            logger.Warning("Serilog | Reaction for such User and Review IDs has been existed");

            return BadRequest(BaseResponse.BadRequest(
                "Reaction for such User and Review IDs has been existed"));
        }

        var reaction = mapper.Map<Reaction>(request);

        dbContext.Reactions.Add(reaction);

        logger.Information("Serilog | Creating reaction...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Created(reaction.Id, "Reaction was created successfully"));
    }

    [HttpPut("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> UpdateReactionV2(int id,
        BaseRequest<ReactionRequest> request)
    {
        logger.Information("Serilog | Getting reaction with ID {Id}...", id);

        var reaction = await dbContext.Reactions.FindAsync(id);

        if (reaction is null)
        {
            logger.Warning("Serilog | Reaction with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("Reaction by ID was not found"));
        }

        if (reaction.IsLike == request.Data.IsLike)
        {
            logger.Warning("Serilog | Reaction with ID {Id} have already had such a reaction", id);

            return BadRequest(
                BaseResponse.BadRequest("Reaction with ID {Id} has already had such a reaction"));
        }

        reaction.IsLike = request.Data.IsLike;

        logger.Information("Serilog | Updating reaction...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Ok("Reaction by ID was updated successfully"));
    }

    [HttpDelete("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> DeleteReactionV2(BaseRequest request, int id)
    {
        logger.Information("Serilog | Getting reaction with ID {Id}...", id);

        var reaction = await dbContext.Reactions.FindAsync(id);

        if (reaction is null)
        {
            logger.Warning("Serilog | Reaction with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("Reaction by ID was not found"));
        }

        dbContext.Reactions.Remove(reaction);

        logger.Information("Serilog | Deleting reaction...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Ok("Reaction by ID was deleted successfully"));
    }
}