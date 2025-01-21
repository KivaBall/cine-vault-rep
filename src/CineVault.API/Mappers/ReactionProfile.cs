using CineVault.API.Controllers.Reactions;

namespace CineVault.API.Mappers;

public sealed class ReactionProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ReactionRequest, Reaction>();
           // .ConstructUsing(r => new Reaction(r.IsLike, r.ReviewId, r.UserId));

        config.NewConfig<Reaction, ReactionResponse>();
    }
}