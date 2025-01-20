namespace CineVault.API.Mappers;

public sealed class ReviewProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ReviewRequest, Review>();

        config.NewConfig<Review, ReviewResponse>()
            .Map(r => r.MovieTitle,
                r => r.Movie!.Title)
            .Map(r => r.Username,
                r => r.User!.Username);
    }
}