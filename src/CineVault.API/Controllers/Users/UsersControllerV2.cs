namespace CineVault.API.Controllers.Users;

public sealed partial class UsersController
{
    [HttpPost("all")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<List<UserResponse>>>> GetUsersV2(
        BaseRequest<GetUsersRequest> request)
    {
        logger.Information("Serilog | Getting users...");

        var query = dbContext.Users.AsQueryable();

        if (request.Data.MinCreatedDate != null)
        {
            query = query.Where(u => u.CreatedAt >= request.Data.MinCreatedDate);
        }

        if (request.Data.MaxCreatedDate != null)
        {
            query = query.Where(u => u.CreatedAt <= request.Data.MaxCreatedDate);
        }

        if (request.Data.SortByAsc != null)
        {
            if ((bool)request.Data.SortByAsc)
            {
                query = query.OrderBy(u => u.CreatedAt);
            }
            else
            {
                query = query.OrderByDescending(u => u.CreatedAt);
            }
        }

        var users = await query
            .Skip(request.Data.Page ?? 0 * request.Data.UsersPerPage ?? 10)
            .Take(request.Data.UsersPerPage ?? 10)
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

    [HttpPost("username")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<UserResponse>>> GetUserByUsernameV2(
        BaseRequest<GetUserByUsernameRequest> request)
    {
        logger.Information("Serilog | Getting user with username {Username}...",
            request.Data.Username);

        var user = await dbContext.Users
            .Where(u => u.Username == request.Data.Username)
            .FirstOrDefaultAsync();

        if (user is null)
        {
            logger.Warning("Serilog | User with username {Username} not found",
                request.Data.Username);

            return NotFound(BaseResponse.NotFound("User by username was not found"));
        }

        var response = mapper.Map<UserResponse>(user);

        return Ok(BaseResponse.Ok(response, "User by username retrieved successfully"));
    }

    [HttpPost("email")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<UserResponse>>> GetUserByEmailV2(
        BaseRequest<GetUserByEmailRequest> request)
    {
        logger.Information("Serilog | Getting user with email {Email}...", request.Data.Email);

        var user = await dbContext.Users
            .Where(u => u.Email == request.Data.Email)
            .FirstOrDefaultAsync();

        if (user is null)
        {
            logger.Warning("Serilog | User with email {Email} not found", request.Data.Email);

            return NotFound(BaseResponse.NotFound("User by email was not found"));
        }

        var response = mapper.Map<UserResponse>(user);

        return Ok(BaseResponse.Ok(response, "User by email retrieved successfully"));
    }

    [HttpPost]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<int>>> CreateUserV2(
        BaseRequest<UserRequest> request)
    {
        var user = mapper.Map<User>(request);

        dbContext.Users.Add(user);

        logger.Information("Serilog | Creating user...");

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Created(user.Id, "User was created successfully"));
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