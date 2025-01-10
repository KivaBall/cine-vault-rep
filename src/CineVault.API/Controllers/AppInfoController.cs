namespace CineVault.API.Controllers;

[Route("api/[action]")]
public sealed class AppInfoController(IHostEnvironment environment) : ControllerBase
{
    [HttpGet]
    public ActionResult<string> Environment()
    {
        return Ok($"Current environment => {environment.EnvironmentName}");
    }

    [HttpGet]
    public ActionResult<string> Exception()
    {
        throw new NotImplementedException("Test exception for Developer Exception Page");
    }
}