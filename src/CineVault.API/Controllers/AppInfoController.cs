namespace CineVault.API.Controllers;

[Route("api/[action]")]
public class AppInfoController(IHostEnvironment environment) : ControllerBase
{
    [HttpGet]
    public ActionResult<string> Environment()
    {
        var environment = configuration["ASPNETCORE_ENVIRONMENT"];

        if (environment == null)
        {
            throw new Exception("Something went wrong?");
        }
            
        return Ok($"Current environment => {environment}");
    }

    [HttpGet]
    public ActionResult<string> Exception()
    {
        throw new Exception("Hello world!");
    }
}