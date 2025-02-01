namespace CineVault.API.Mappers;

public sealed class UserProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserRequest, User>()
            .MapWith(u => new User(u.Username, u.Email, u.Password));

        config.NewConfig<User, UserResponse>();

        config.NewConfig<User, UserStats>()
            .Map(u => u.ReviewCount,
                u => u.Reviews.Count)
            .Map(u => u.AverageRating,
                u => u.Reviews.Count != 0 ? u.Reviews.Average(r => r.Rating) : 0)
            .Map(u => u.PopularUserGenres,
                u => u.Reviews
                    .Where(r => r.Movie.Genre != null)
                    .GroupBy(r => r.Movie.Genre)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => g.Key)
                    .ToList())
            .Map(u => u.LastActivity,
                u => u.Reviews.Any()
                    ? u.Reviews.Max(r => r.CreatedAt)
                    : u.CreatedAt);
    }
}