namespace CineVault.API.Controllers.Users;

public sealed partial class UsersController(
    CineVaultDbContext dbContext,
    ILogger logger,
    IMapper mapper)
    : BaseController
{
    [HttpGet]
    [MapToApiVersion(1)]
    public async Task<ActionResult<List<UserResponse>>> GetUsersV1()
    {
        logger.Information("Serilog | Getting users...");

        var users = await dbContext.Users
            .Select(u => mapper.Map<UserResponse>(u))
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("{id:int}")]
    [MapToApiVersion(1)]
    public async Task<ActionResult<UserResponse>> GetUserByIdV1(int id)
    {
        logger.Information("Serilog | Getting user with ID {Id}...", id);

        var user = await dbContext.Users.FindAsync(id);

        if (user is null)
        {
            logger.Warning("Serilog | User with ID {Id} not found", id);

            return NotFound();
        }

        var response = mapper.Map<UserResponse>(user);

        return Ok(response);
    }

    [HttpPost]
    [MapToApiVersion(1)]
    public async Task<ActionResult> CreateUserV1(UserRequest request)
    {
        var user = mapper.Map<User>(request);

        dbContext.Users.Add(user);

        logger.Information("Serilog | Creating user...");

        await dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPut("{id:int}")]
    [MapToApiVersion(1)]
    public async Task<ActionResult> UpdateUserV1(int id, UserRequest request)
    {
        logger.Information("Serilog | Getting user with ID {Id}...", id);

        var user = await dbContext.Users.FindAsync(id);

        if (user is null)
        {
            logger.Warning("Serilog | User with ID {Id} not found", id);

            return NotFound();
        }

        user.Username = request.Username;
        user.Email = request.Email;
        user.Password = request.Password;

        logger.Information("Serilog | Updating user...");

        await dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{id:int}")]
    [MapToApiVersion(1)]
    public async Task<ActionResult> DeleteUserV1(int id)
    {
        logger.Information("Serilog | Getting user with ID {Id}...", id);

        var user = await dbContext.Users.FindAsync(id);

        if (user is null)
        {
            logger.Warning("Serilog | User with ID {Id} not found", id);

            return NotFound();
        }

        dbContext.Users.Remove(user);

        logger.Information("Serilog | Deleting user...");

        await dbContext.SaveChangesAsync();

        return Ok();
    }
}