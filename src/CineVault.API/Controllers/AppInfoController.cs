namespace CineVault.API.Controllers;

[Route("api/[action]")]
public class AppInfoController(IHostEnvironment environment) : ControllerBase
{
    [HttpGet]
    public ActionResult<string> Environment()
    {
        return Ok($"Current environment => {environment.EnvironmentName}");
    }

    [HttpGet]
    public ActionResult<string> Exception()
    {
        throw new Exception("Hello world!");
    }
}