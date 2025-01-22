namespace CineVault.API.Mappers;

public sealed class ReviewProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ReviewRequest, Review>()
            .MapWith(r => new Review(r.Comment, r.Rating, r.MovieId, r.UserId));

        config.NewConfig<Review, ReviewResponse>()
            .Map(r => r.MovieTitle,
                r => r.Movie!.Title)
            .Map(r => r.Username,
                r => r.User!.Username)
            .Map(r => r.Likes,
                r => r.Reactions.Count(r2 => r2.IsLike))
            .Map(r => r.Dislikes,
                r => r.Reactions.Count(r2 => !r2.IsLike));
    }
}