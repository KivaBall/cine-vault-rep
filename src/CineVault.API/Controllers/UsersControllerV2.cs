using Asp.Versioning;
using CineVault.API.Abstractions.Controllers;
using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Entities;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineVault.API.Controllers;

public sealed partial class UsersController
{
    private static readonly Func<CineVaultDbContext, string, string, Task<UserCheckResult?>>
        GetUserCheck = EF.CompileAsyncQuery(
            (CineVaultDbContext context, string username, string email) =>
                context.Users
                    .AsNoTracking()
                    .Where(u => u.Username == username || u.Email == email)
                    .Select(u => new UserCheckResult(
                        u.Username == username,
                        u.Email == email
                    ))
                    .FirstOrDefault());

    [HttpPost("all")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<List<UserResponse>>>> GetUsersV2(
        BaseRequest<GetUsersRequest> request)
    {
        logger.Information("Serilog | Getting users...");

        var users = await dbContext.Users
            .AsNoTracking()
            .Where(u =>
                request.Data.MinCreatedDate == null
                || u.CreatedAt >= request.Data.MinCreatedDate)
            .Where(u =>
                request.Data.MaxCreatedDate == null
                || u.CreatedAt <= request.Data.MaxCreatedDate)
            .OrderBy(u =>
                request.Data.SortByAsc == true ? u.CreatedAt.Ticks :
                request.Data.SortByAsc == false ? -u.CreatedAt.Ticks : 0)
            .Skip((request.Data.UsersPerPage ?? 10) * ((request.Data.Page ?? 1) - 1))
            .Take(request.Data.UsersPerPage ?? 10)
            .ProjectToType<UserResponse>()
            .ToListAsync();

        return Ok(BaseResponse.Ok(users, "Users retrieved successfully"));
    }

    [HttpPost("{id:int}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<UserResponse>>> GetUserByIdV2(BaseRequest request,
        int id)
    {
        logger.Information("Serilog | Getting user with ID {Id}...", id);

        var user = await dbContext.Users
            .AsNoTracking()
            .ProjectToType<UserResponse>()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            logger.Warning("Serilog | User with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("User by ID was not found"));
        }

        return Ok(BaseResponse.Ok(user, "User by ID retrieved successfully"));
    }

    [HttpPost("username")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<UserResponse>>> GetUserByUsernameV2(
        BaseRequest<string> request)
    {
        logger.Information("Serilog | Getting user with username {Username}...", request.Data);

        var user = await dbContext.Users
            .AsNoTracking()
            .ProjectToType<UserResponse>()
            .FirstOrDefaultAsync(u => u.Username == request.Data);

        if (user == null)
        {
            logger.Warning("Serilog | User with username {Username} not found", request.Data);

            return NotFound(BaseResponse.NotFound("User by username was not found"));
        }

        return Ok(BaseResponse.Ok(user, "User by username retrieved successfully"));
    }

    [HttpPost("email")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<UserResponse>>> GetUserByEmailV2(
        BaseRequest<string> request)
    {
        logger.Information("Serilog | Getting user with email {Email}...", request.Data);

        var user = await dbContext.Users
            .AsNoTracking()
            .ProjectToType<UserResponse>()
            .FirstOrDefaultAsync(u => u.Email == request.Data);

        if (user == null)
        {
            logger.Warning("Serilog | User with email {Email} not found", request.Data);

            return NotFound(BaseResponse.NotFound("User by email was not found"));
        }

        return Ok(BaseResponse.Ok(user, "User by email retrieved successfully"));
    }

    [HttpPost("{id:int}/user-stats")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<UserStats>>> GetUserStatsV2(BaseRequest request,
        int id)
    {
        logger.Information("Serilog | Getting user with ID {Id}...", id);

        var user = await dbContext.Users
            .AsNoTracking()
            .ProjectToType<UserStats>()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            logger.Warning("Serilog | User with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("User by ID was not found"));
        }

        return Ok(BaseResponse.Ok(user, "User stats by ID retrieved successfully"));
    }

    [HttpPost]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<int>>> CreateUserV2(
        BaseRequest<UserRequest> request)
    {
        var data = await GetUserCheck(dbContext, request.Data.Username, request.Data.Email);

        if (data != null)
        {
            if (data.UsernameExists)
            {
                logger.Warning("Serilog | Username '{Username}' already exists",
                    request.Data.Username);

                return BadRequest(BaseResponse.BadRequest("Username is already in use"));
            }

            if (data.EmailExists)
            {
                logger.Warning("Serilog | Email '{Email}' already exists", request.Data.Email);

                return BadRequest(BaseResponse.BadRequest("Email is already in use"));
            }
        }

        var user = mapper.Map<User>(request.Data);

        dbContext.Users.Add(user);

        logger.Information("Serilog | Creating user...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Created(user.Id, "User was created successfully"));
    }

    [HttpPut("{id:int}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> UpdateUserV2(int id,
        BaseRequest<UserRequest> request)
    {
        logger.Information("Serilog | Getting user with ID {Id}...", id);

        var user = await dbContext.Users.FindAsync(id);

        if (user == null)
        {
            logger.Warning("Serilog | User with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("User by ID was not found"));
        }

        var data = await dbContext.Users
            .AsNoTracking()
            .Where(u =>
                (u.Username == request.Data.Username || u.Email == request.Data.Email) &&
                u.Id != id)
            .Select(u => new
            {
                UsernameExists = u.Username == request.Data.Username,
                EmailExists = u.Email == request.Data.Email
            })
            .FirstOrDefaultAsync();

        if (data != null)
        {
            if (data.UsernameExists)
            {
                logger.Warning("Serilog | Username '{Username}' already exists",
                    request.Data.Username);

                return BadRequest(BaseResponse.BadRequest("Username is already in use"));
            }

            if (data.EmailExists)
            {
                logger.Warning("Serilog | Email '{Email}' already exists", request.Data.Email);

                return BadRequest(BaseResponse.BadRequest("Email is already in use"));
            }
        }

        user.Username = request.Data.Username;
        user.Email = request.Data.Email;
        user.Password = request.Data.Password;

        logger.Information("Serilog | Updating user...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Ok("User by ID was updated successfully"));
    }

    [HttpDelete("{id:int}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> DeleteUserV2(BaseRequest request, int id)
    {
        logger.Information("Serilog | Getting user with ID {Id}...", id);

        var user = await dbContext.Users.FindAsync(id);

        if (user is null)
        {
            logger.Warning("Serilog | User with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("User by ID was not found"));
        }

        user.IsDeleted = true;

        dbContext.Users.Update(user);

        logger.Information("Serilog | Deleting user...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Ok("User by ID was deleted successfully"));
    }

    [HttpPut("{id:int}/restore")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> RestoreUserV2(BaseRequest request, int id)
    {
        logger.Information("Serilog | Getting deleted user with ID {Id}...", id);

        var user = await dbContext.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            logger.Warning("Serilog | User with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("User by ID was not found"));
        }

        if (!user.IsDeleted)
        {
            logger.Warning("Serilog | User with ID {Id} not deleted", id);

            return BadRequest(BaseResponse.BadRequest("User by ID was not deleted"));
        }

        user.IsDeleted = false;

        dbContext.Users.Update(user);

        logger.Information("Serilog | Restoring user...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Ok("User by ID was restored successfully"));
    }

    private record UserCheckResult(bool UsernameExists, bool EmailExists);
}