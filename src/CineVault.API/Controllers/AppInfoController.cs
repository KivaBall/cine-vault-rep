namespace CineVault.API.Controllers;

public sealed class AppInfoController(IHostEnvironment environment) : BaseController
{
    [HttpGet("environment")]
    [MapToApiVersion(1)]
    public ActionResult<string> EnvironmentV1()
    {
        return Ok($"Current environment => {environment.EnvironmentName}");
    }

    [HttpGet("environment")]
    [MapToApiVersion(2)]
    public ActionResult<string> EnvironmentV2()
    {
        return Ok($"""
                   - - - 
                   Current environment => {environment.EnvironmentName}
                   - - -
                   In application => {environment.ApplicationName}
                   - - -
                   Is development: {environment.IsDevelopment()}
                   - - -
                   Is production: {environment.IsProduction()}
                   """);
    }

    [HttpGet("exception")]
    public ActionResult<string> Exception()
    {
        throw new NotImplementedException("Test exception for Developer Exception Page");
    }

    [HttpGet("old")]
    [MapToApiVersion(1)]
    public ActionResult<string> Old()
    {
        return Ok("""☠ I am an old endpoint for only v1 ☠""");
    }

    [HttpGet("yahoo")]
    [MapToApiVersion(2)]
    public ActionResult<string> Yahoo()
    {
        return Ok("""
                  Yahoo
                        ooooooo
                                ooooo!
                                
                  ~ ~ I am a new endpoint supported only in v2! ~ ~
                  """);
    }
}