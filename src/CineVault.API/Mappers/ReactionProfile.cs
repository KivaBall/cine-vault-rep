using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Entities;
using Mapster;

namespace CineVault.API.Mappers;

public sealed class ReactionProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ReactionRequest, Reaction>()
            .MapWith(r => new Reaction(r.IsLike, r.ReviewId, r.UserId));

        config.NewConfig<Reaction, ReactionResponse>();
    }
}