namespace CineVault.API.Extensions;

public static class PipelineExtensions
{
    public static void BuildPipeline(this WebApplication app)
    {
        app.UseSwaggerMiddleware();

        if (app.Environment.IsLocal())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseMiddleware<SerilogMiddleware>();

        app.MapControllers();

        Console.WriteLine($"Current environment is {app.Environment.EnvironmentName}");
    }

    private static void UseSwaggerMiddleware(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return;
        }

        app.UseSwagger();

        app.UseSwaggerUI(opt =>
        {
            opt.SwaggerEndpoint("v1/swagger.json", "CineVault 'Gray' API V1");

            opt.SwaggerEndpoint("v2/swagger.json", "CineVault 'KitKat' API V2");
        });
    }
}