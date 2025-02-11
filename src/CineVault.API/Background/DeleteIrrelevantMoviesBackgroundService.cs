using CineVault.API.Entities;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace CineVault.API.Background;

public class DeleteIrrelevantMoviesBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            logger.Information(
                "Serilog | Executing background service for deleting irrelevant movies");

            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CineVaultDbContext>();

            var threeYearsAgo = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-3));
            var twoYearsAgo = DateTime.UtcNow.AddYears(-2);

            var movies = await context.Movies
                .Where(m =>
                    (m.Reviews.Count == 0 && m.ReleaseDate!.Value < threeYearsAgo)
                    || (m.Reviews.Any() && m.Reviews.Max(r => r.CreatedAt) < twoYearsAgo))
                .ToListAsync(token);

            movies.ForEach(m => m.IsDeleted = true);

            await context.SaveChangesAsync(token);

            logger.Information(
                "Serilog | Finished executing background service for deleting irrelevant movies. " +
                "Movies deleted: {DeletedCount}", movies.Count);

            await Task.Delay(TimeSpan.FromSeconds(45), token);
        }
    }
}