namespace CineVault.API.Controllers;

public sealed partial class UsersController(CineVaultDbContext dbContext) : BaseController
{
    [HttpGet]
    [MapToApiVersion(1)]
    public async Task<ActionResult<List<UserResponse>>> GetUsersV1()
    {
        var users = await dbContext.Users
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("{id}")]
    [MapToApiVersion(1)]
    public async Task<ActionResult<UserResponse>> GetUserByIdV1(int id)
    {
        var user = await dbContext.Users.FindAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        var response = new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };

        return Ok(response);
    }

    [HttpPost]
    [MapToApiVersion(1)]
    public async Task<ActionResult> CreateUserV1(UserRequest request)
    {
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password
        };

        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPut("{id}")]
    [MapToApiVersion(1)]
    public async Task<ActionResult> UpdateUserV1(int id, UserRequest request)
    {
        var user = await dbContext.Users.FindAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        user.Username = request.Username;
        user.Email = request.Email;
        user.Password = request.Password;

        await dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{id}")]
    [MapToApiVersion(1)]
    public async Task<ActionResult> DeleteUserV1(int id)
    {
        var user = await dbContext.Users.FindAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        dbContext.Users.Remove(user);

        await dbContext.SaveChangesAsync();

        return Ok();
    }
}