using Asp.Versioning;
using CineVault.API.Abstractions.Controllers;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace CineVault.API.Controllers;

public sealed class AppInfoController(
    IHostEnvironment environment,
    ILogger logger)
    : BaseController
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