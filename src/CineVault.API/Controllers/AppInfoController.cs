namespace CineVault.API.Controllers;

public sealed class AppInfoController(IHostEnvironment environment, ILogger logger) : BaseController
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

    [HttpGet("logging_test")]
    public ActionResult<string> LoggingTest()
    {
        logger.Verbose("Verbose test");

        logger.Debug("Debug test");

        logger.Information("Information test");

        logger.Warning("Warning test");

        logger.Error("Error test");

        logger.Fatal("Fatal test");

        return Ok("Logger was tested");
    }
}