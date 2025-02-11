using CineVault.API.Entities;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace CineVault.API.Background;

// TODO a) фоновий метод для періодичного оновлення статистики по фільмах
public sealed class MovieStatsBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            logger.Information("Serilog | Executing background service for movie stats");

            // TODO a) кожний раз процес запускає перевірку в бд і калькулює статистику фільмів
            // (колонки фільм-id, середня оцінка, кількість відгуків, коментів по кожному фільму,
            // ознака чи видалений фільм (deleted), дата зміни рядка статистики). 
            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CineVaultDbContext>();

            var movies = await context.Movies
                .IgnoreQueryFilters()
                .Include(m => m.Reviews)
                .ToListAsync(token);

            var movieIds = movies.Select(m => m.Id).ToList();

            var movieStatsList = await context.MovieStats
                .Where(ms => movieIds.Contains(ms.MovieId))
                .ToListAsync(token);

            var movieStatsDictionary = movieStatsList.ToDictionary(ms => ms.MovieId);

            var now = DateTime.UtcNow;

            var addedCount = 0;
            var updatedCount = 0;

            foreach (var movie in movies)
            {
                var avgRating = movie.Reviews.Any() ? movie.Reviews.Average(r => r.Rating) : 0;
                var reviewCount = movie.Reviews.Count;
                var isDeleted = movie.IsDeleted;

                // TODO a) якщо таблиця пуста, записуються нові записи по статистиці, якщо
                // інформація по фільму для статистики змінюється – виконується оновлення полів статистики
                if (!movieStatsDictionary.TryGetValue(movie.Id, out var existingStat))
                {
                    var newStat = new MovieStat(movie.Id, avgRating, reviewCount, isDeleted)
                    {
                        LastChangedAt = now
                    };

                    await context.MovieStats.AddAsync(newStat, token);

                    addedCount++;

                    logger.Information("Serilog | Added movie stats for ID {MovieId}", movie.Id);
                }
                else if (existingStat.AverageRating != avgRating ||
                         existingStat.ReviewCount != reviewCount ||
                         existingStat.MovieWasDeleted != isDeleted)
                {
                    existingStat.AverageRating = avgRating;
                    existingStat.ReviewCount = reviewCount;
                    existingStat.MovieWasDeleted = isDeleted;
                    existingStat.LastChangedAt = now;

                    updatedCount++;

                    logger.Information("Serilog | Updated movie stats for ID {MovieId}", movie.Id);
                }
            }

            await context.SaveChangesAsync(token);

            // TODO c) зробити логування в консоль успішного виконання кожного з методів з інформацією
            // про кількість видалених, оновлених записів, нових записів в таблицю кожний показник окремо
            logger.Information(
                "Serilog | Finished executing background service for movie stats. " +
                "Movie stats added: {AddedCount}, movie stats updated: {UpdatedCount}",
                addedCount, updatedCount);

            // TODO a) процес перевірки повинен виконуватись кожні 30 сек
            await Task.Delay(TimeSpan.FromSeconds(30), token);
        }
    }
}