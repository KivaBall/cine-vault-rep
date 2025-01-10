namespace CineVault.API.Controllers;

[Route("api/[controller]/[action]")]
public sealed class UsersController : ControllerBase
{
    private readonly CineVaultDbContext _dbContext;

    public UsersController(CineVaultDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> GetUsers()
    {
        var users = await _dbContext.Users
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
    public async Task<ActionResult<UserResponse>> GetUserById(int id)
    {
        var user = await _dbContext.Users.FindAsync(id);

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
    public async Task<ActionResult> CreateUser(UserRequest request)
    {
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateUser(int id, UserRequest request)
    {
        var user = await _dbContext.Users.FindAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        user.Username = request.Username;
        user.Email = request.Email;
        user.Password = request.Password;

        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var user = await _dbContext.Users.FindAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }
}