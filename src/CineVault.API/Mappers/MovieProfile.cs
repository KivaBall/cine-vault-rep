using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Entities;
using Mapster;

namespace CineVault.API.Mappers;

public sealed class MovieProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<MovieRequest, Movie>()
            .MapWith(m => new Movie(m.Title, m.Description, m.ReleaseDate, m.Genre, m.Director));

        config.NewConfig<Movie, MovieResponse>()
            .Map(m => m.AverageRating,
                m => m.Reviews.Count != 0 ? m.Reviews.Average(r => r.Rating) : 0)
            .Map(m => m.ReviewCount,
                m => m.Reviews.Count);

        config.NewConfig<Movie, MovieDetails>()
            .Map(m => m.AverageRating,
                m => m.Reviews.Count != 0 ? m.Reviews.Average(r => r.Rating) : 0)
            .Map(m => m.ReviewCount,
                m => m.Reviews.Count)
            .Map(m => m.LastReviews,
                m => m.Reviews
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(5));
    }
}