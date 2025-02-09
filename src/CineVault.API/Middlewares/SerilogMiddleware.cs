using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Controllers;
using ILogger = Serilog.ILogger;

namespace CineVault.API.Middlewares;

public sealed class SerilogMiddleware(ILogger logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            var endpoint = context.GetEndpoint();

            string? controllerName = null;
            string? actionName = null;

            var controllerActionDescriptor =
                endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

            if (controllerActionDescriptor != null)
            {
                controllerName = controllerActionDescriptor.ControllerName;
                actionName = controllerActionDescriptor.ActionName;
            }

            logger.Information(
                "Serilog | Handling request: {Path} for controller {ControllerName} with method {Method}",
                context.Request.Path, controllerName ?? "UNKNOWN", actionName ?? "UNKNOWN");

            var stopwatch = Stopwatch.StartNew();

            await next(context);

            logger.Information("Serilog | Handled request: {Path}, for {Milliseconds} milliseconds",
                context.Request.Path, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            logger.Error("Serilog | Something went wrong! Exception: {Exception}", ex.ToString());
        }
    }
}