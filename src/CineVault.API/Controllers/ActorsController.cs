using Asp.Versioning;
using CineVault.API.Abstractions.Controllers;
using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Entities;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace CineVault.API.Controllers;

// TODO 5 Реалізувати контролер та основні методи CRUD для нової сутності
public sealed class ActorsController(
    CineVaultDbContext dbContext,
    ILogger logger,
    IMapper mapper)
    : BaseController
{
    [HttpPost("all")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<ICollection<ActorResponse>>>> GetActorsV2(
        BaseRequest request)
    {
        logger.Information("Serilog | Getting actors ...");

        // TODO 13 Визначити, де у вашому проєкті використовуються запити лише для читання даних, та додати AsNoTracking до них
        // TODO 13 Проаналізувати, чи не додаєте ви зайвих Include у запитах
        var actors = await dbContext.Actors
            .AsNoTracking()
            .ProjectToType<ActorResponse>()
            .ToListAsync();

        return Ok(BaseResponse.Ok(actors, "Actors retrieved successfully"));
    }

    [HttpPost("{id:int}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<ActorResponse>>> GetActorByIdV2(BaseRequest request,
        int id)
    {
        logger.Information("Serilog | Getting actor with ID {Id}...", id);

        // TODO 13 Визначити, де у вашому проєкті використовуються запити лише для читання даних, та додати AsNoTracking до них
        // TODO 13 Проаналізувати, чи не додаєте ви зайвих Include у запитах
        var actor = await dbContext.Actors
            .AsNoTracking()
            .ProjectToType<ActorResponse>()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (actor == null)
        {
            logger.Warning("Serilog | Actor with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("Actor by ID was not found"));
        }

        return Ok(BaseResponse.Ok(actor, "Actor by ID retrieved successfully"));
    }

    [HttpPost]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<int>>> CreateActorV2(
        BaseRequest<ActorRequest> request)
    {
        var fullNameExists = await dbContext.Actors
            .AnyAsync(a => a.FullName == request.Data.FullName);

        if (fullNameExists)
        {
            logger.Warning("Serilog | Actor with FullName '{FullName}' already exists",
                request.Data.FullName);

            return BadRequest(BaseResponse.BadRequest("Actor with this FullName already exists"));
        }

        var actor = mapper.Map<Actor>(request.Data);

        dbContext.Actors.Add(actor);

        logger.Information("Serilog | Creating actor...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Created(actor.Id, "Actor was created successfully"));
    }

    // TODO 5 Додати метод для завантаження одразу масиву даних з акторами
    [HttpPost("several")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<ICollection<int>>>> CreateActorsV2(
        BaseRequest<ICollection<ActorRequest>> request)
    {
        var requestedFullNames = request.Data.Select(a => a.FullName).ToList();

        // TODO 13 Визначити, де у вашому проєкті використовуються запити лише для читання даних, та додати AsNoTracking до них
        var existingFullNames = await dbContext.Actors
            .AsNoTracking()
            .Where(a => requestedFullNames.Contains(a.FullName))
            .Select(a => a.FullName)
            .ToListAsync();

        var duplicateFullNames = requestedFullNames.Intersect(existingFullNames).ToList();

        if (duplicateFullNames.Count != 0)
        {
            logger.Warning("Serilog | Some actor FullNames already exist: {FullNames}",
                string.Join(", ", duplicateFullNames));

            return BadRequest(BaseResponse.BadRequest(
                $"Some actor FullNames already exist: {string.Join(", ", duplicateFullNames)}"));
        }

        var actors = request.Data
            .Select(mapper.Map<Actor>)
            .ToList();

        dbContext.Actors.AddRange(actors);

        logger.Information("Serilog | Creating actors...");

        await dbContext.SaveChangesAsync();

        var ids = actors
            .Select(a => a.Id)
            .ToList();

        return Ok(BaseResponse.Created(ids, "Actor was created successfully"));
    }

    [HttpPut("{id:int}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> UpdateActorV2(int id,
        BaseRequest<ActorRequest> request)
    {
        logger.Information("Serilog | Getting actor with ID {Id}...", id);

        var actor = await dbContext.Actors.FindAsync(id);

        if (actor == null)
        {
            logger.Warning("Serilog | Actor with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("Actor by ID was not found"));
        }

        var fullNameExists = await dbContext.Actors
            .AnyAsync(a => a.FullName == request.Data.FullName && a.Id != id);

        if (fullNameExists)
        {
            logger.Warning("Serilog | Actor with FullName '{FullName}' already exists",
                request.Data.FullName);

            return BadRequest(BaseResponse.BadRequest("Actor with this FullName already exists"));
        }

        actor.FullName = request.Data.FullName;
        actor.BirthDate = request.Data.BirthDate;
        actor.Biography = request.Data.Biography;

        logger.Information("Serilog | Updating actor...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Ok("Actor by ID was updated successfully"));
    }

    [HttpDelete("{id:int}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> DeleteActorV2(BaseRequest request, int id)
    {
        logger.Information("Serilog | Getting actor with ID {Id}...", id);

        var actor = await dbContext.Actors.FindAsync(id);

        if (actor == null)
        {
            logger.Warning("Serilog | Actor with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("Actor by ID was not found"));
        }

        actor.IsDeleted = true;

        dbContext.Actors.Update(actor);

        logger.Information("Serilog | Deleting actor...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Ok("Actor by ID was deleted successfully"));
    }
}