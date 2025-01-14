namespace CineVault.API.Controllers;

public sealed class AppInfoController(IHostEnvironment environment) : BaseController
{
    [HttpGet("environment")]
    public ActionResult<string> Environment()
    {
        return Ok($"Current environment => {environment.EnvironmentName}");
    }

    [HttpGet("exception")]
    public ActionResult<string> Exception()
    {
        throw new NotImplementedException("Test exception for Developer Exception Page");
    }
}