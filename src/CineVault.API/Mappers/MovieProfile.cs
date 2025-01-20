namespace CineVault.API.Mappers;

public sealed class MovieProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<MovieRequest, Movie>();

        config.NewConfig<Movie, MovieResponse>()
            .Map(m => m.AverageRating,
                m => m.Reviews.Count != 0 ? m.Reviews.Average(r => r.Rating) : 0)
            .Map(m => m.ReviewCount,
                m => m.Reviews.Count);
    }
}