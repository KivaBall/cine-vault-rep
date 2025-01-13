namespace CineVault.API.Middlewares;

public sealed class SerilogMiddleware(ILogger logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            logger.Information("Serilog | Handling request: {Path}", context.Request.Path);

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