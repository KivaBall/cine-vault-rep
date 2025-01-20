namespace CineVault.API.Controllers;

public sealed partial class UsersController
{
    [HttpPost("all")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<List<UserResponse>>>> GetUsersV2(
        BaseRequest request)
    {
        logger.Information("Serilog | Getting users...");

        var users = await dbContext.Users
            .Select(u => mapper.Map<UserResponse>(u))
            .ToListAsync();

        return Ok(BaseResponse.Ok(users, "Users retrieved successfully"));
    }

    [HttpPost("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<UserResponse>>> GetUserByIdV2(BaseRequest request,
        int id)
    {
        logger.Information("Serilog | Getting user with ID {Id}...", id);

        var user = await dbContext.Users.FindAsync(id);

        if (user is null)
        {
            logger.Warning("Serilog | User with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("User by ID was not found"));
        }

        var response = mapper.Map<UserResponse>(user);

        return Ok(BaseResponse.Ok(response, "User by ID retrieved successfully"));
    }

    [HttpPost]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> CreateUserV2(BaseRequest<UserRequest> request)
    {
        var user = new User
        {
            Username = request.Data.Username,
            Email = request.Data.Email,
            Password = request.Data.Password
        };

        dbContext.Users.Add(user);

        logger.Information("Serilog | Creating user...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Ok("User was created successfully"));
    }

    [HttpPut("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> UpdateUserV2(int id,
        BaseRequest<UserRequest> request)
    {
        logger.Information("Serilog | Getting user with ID {Id}...", id);

        var user = await dbContext.Users.FindAsync(id);

        if (user is null)
        {
            logger.Warning("Serilog | User with ID {Id} not found", id);

            return NotFound(BaseResponse.NotFound("User by ID was not found"));
        }

        user.Username = request.Data.Username;
        user.Email = request.Data.Email;
        user.Password = request.Data.Password;

        logger.Information("Serilog | Updating user...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Ok("User by ID was updated successfully"));
    }

    [HttpDelete("{id}")]
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

        dbContext.Users.Remove(user);

        logger.Information("Serilog | Deleting user...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Ok("User by ID was deleted successfully"));
    }
}