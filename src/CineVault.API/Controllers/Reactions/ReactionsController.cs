namespace CineVault.API.Controllers.Reactions;

public sealed class ReactionsController(
    CineVaultDbContext dbContext,
    ILogger logger,
    IMapper mapper)
    : BaseController
{
    [HttpPost("{id:int}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<ReactionResponse>>> GetReactionByIdV2(
        BaseRequest request, int id)
    {
        logger.Information("Serilog | Getting reaction with ID {Id}...", id);

        // TODO 13 Визначити, де у вашому проєкті використовуються запити лише для читання даних, та додати AsNoTracking до них
        // TODO 13 Проаналізувати, чи не додаєте ви зайвих Include у запитах
        var reaction = await dbContext.Reactions
            .AsNoTracking()
            .ProjectToType<ReactionResponse>()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reaction == null)
        {
            logger.Warning("Serilog | Reaction with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("Reaction by ID was not found"));
        }

        return Ok(BaseResponse.Ok(reaction, "Reaction by ID retrieved successfully"));
    }

    [HttpPost]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<int>>> CreateReactionV2(
        BaseRequest<ReactionRequest> request)
    {
        // TODO 13 Визначити, де у вашому проєкті використовуються запити лише для читання даних, та додати AsNoTracking до них
        // TODO 13 Оптимізуйте місця в коді, де виникають кілька запитів на отримання даних, об'єднавши їх у один запит
        var data = await dbContext.Reviews
            .AsNoTracking()
            .Where(r => r.Id == request.Data.ReviewId)
            .Select(r => new
            {
                UserExists = dbContext.Users.Any(u => u.Id == request.Data.UserId),
                ReactionExists = dbContext.Reactions.Any(r2 =>
                    r2.ReviewId == request.Data.ReviewId && r2.UserId == request.Data.UserId)
            })
            .FirstOrDefaultAsync();

        if (data == null)
        {
            logger.Warning("Serilog | Specified review ID cannot be found");

            return BadRequest(BaseResponse.BadRequest("Specified review ID cannot be found"));
        }

        if (!data.UserExists)
        {
            logger.Warning("Serilog | Specified user ID cannot be found");

            return BadRequest(BaseResponse.BadRequest("Specified user ID cannot be found"));
        }

        if (!data.ReactionExists)
        {
            logger.Warning("Serilog | Reaction for such User and Review IDs has been existed");

            return BadRequest(
                BaseResponse.BadRequest("Reaction for such User and Review IDs has been existed"));
        }

        var reaction = mapper.Map<Reaction>(request.Data);

        dbContext.Reactions.Add(reaction);

        logger.Information("Serilog | Creating reaction...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Created(reaction.Id, "Reaction was created successfully"));
    }

    [HttpPut("{id:int}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> UpdateReactionV2(int id,
        BaseRequest<ReactionRequest> request)
    {
        logger.Information("Serilog | Getting reaction with ID {Id}...", id);

        var reaction = await dbContext.Reactions.FindAsync(id);

        if (reaction == null)
        {
            logger.Warning("Serilog | Reaction with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("Reaction by ID was not found"));
        }

        if (reaction.IsLike == request.Data.IsLike)
        {
            logger.Warning("Serilog | Reaction with ID {Id} have already had such a reaction", id);

            return BadRequest(
                BaseResponse.BadRequest("Reaction with ID has already had such a reaction"));
        }

        reaction.IsLike = request.Data.IsLike;

        logger.Information("Serilog | Updating reaction...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Ok("Reaction by ID was updated successfully"));
    }

    [HttpDelete("{id:int}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> DeleteReactionV2(BaseRequest request, int id)
    {
        logger.Information("Serilog | Getting reaction with ID {Id}...", id);

        // TODO 13 Визначити, де у вашому проєкті використовуються запити лише для читання даних, та додати AsNoTracking до них
        var reaction = await dbContext.Reactions.FindAsync(id);

        if (reaction == null)
        {
            logger.Warning("Serilog | Reaction with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("Reaction by ID was not found"));
        }

        reaction.IsDeleted = true;
        
        dbContext.Reactions.Update(reaction);

        logger.Information("Serilog | Deleting reaction...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Ok("Reaction by ID was deleted successfully"));
    }
}