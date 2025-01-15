namespace CineVault.API.Controllers;

public sealed partial class UsersController
{
    [HttpGet]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<List<UserResponse>>>> GetUsersV2()
    {
        var users = await dbContext.Users
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email
            })
            .ToListAsync();

        return Ok(BaseResponse.Ok(users, "Users retrieved successfully"));
    }

    [HttpGet("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse<UserResponse>>> GetUserByIdV2(int id)
    {
        var user = await dbContext.Users.FindAsync(id);

        if (user is null)
        {
            return NotFound(BaseResponse.NotFound("User by ID was not found"));
        }

        var response = new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };

        return Ok(BaseResponse.Ok(response, "User by ID retrieved successfully"));
    }

    [HttpPost]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> CreateUserV2(UserRequest request)
    {
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password
        };

        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Ok("User was created successfully"));
    }

    [HttpPut("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> UpdateUserV2(int id, UserRequest request)
    {
        var user = await dbContext.Users.FindAsync(id);

        if (user is null)
        {
            return NotFound(BaseResponse.NotFound("User by ID was not found"));
        }

        user.Username = request.Username;
        user.Email = request.Email;
        user.Password = request.Password;

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Ok("User by ID was updated successfully"));
    }

    [HttpDelete("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<BaseResponse>> DeleteUserV2(int id)
    {
        var user = await dbContext.Users.FindAsync(id);

        if (user is null)
        {
            return NotFound(BaseResponse.NotFound("User by ID was not found"));
        }

        dbContext.Users.Remove(user);

        await dbContext.SaveChangesAsync();

        return Ok(BaseResponse.Ok("User by ID was deleted successfully"));
    }
}