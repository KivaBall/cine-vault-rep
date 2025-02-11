using CineVault.API.Entities;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace CineVault.API.Background;

// TODO b) фоновий метод видалення фільмів з таблиці 
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

            // TODO b) з застарілими відгуками (останній відгук залишений більше 2 років тому),
            // або їх відсутність з початку релізу фільму (з дня релізу пройшло більше 3 років)
            var movies = await context.Movies
                .Where(m =>
                    (m.Reviews.Count == 0 && m.ReleaseDate!.Value < threeYearsAgo)
                    || (m.Reviews.Any() && m.Reviews.Max(r => r.CreatedAt) < twoYearsAgo))
                .ToListAsync(token);

            movies.ForEach(m => m.IsDeleted = true);

            await context.SaveChangesAsync(token);

            // TODO c) зробити логування в консоль успішного виконання кожного з методів з інформацією
            // про кількість видалених, оновлених записів, нових записів в таблицю кожний показник окремо
            logger.Information(
                "Serilog | Finished executing background service for deleting irrelevant movies. " +
                "Movies deleted: {DeletedCount}", movies.Count);

            // TODO b) кожні 45 сек 
            await Task.Delay(TimeSpan.FromSeconds(45), token);
        }
    }
}