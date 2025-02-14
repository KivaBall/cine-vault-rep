using CineVault.API.Entities;

namespace CineVault.API.Extensions;

public static class PipelineExtensions
{
    public static void UseSwaggerMiddleware(this WebApplication app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(opt =>
        {
            opt.SwaggerEndpoint("v1/swagger.json", "CineVault 'Gray' API V1");

            opt.SwaggerEndpoint("v2/swagger.json", "CineVault 'KitKat' API V2");
        });
    }

    public static void ConfigureDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<CineVaultDbContext>();

        dbContext.Database.EnsureCreated();
    }
}