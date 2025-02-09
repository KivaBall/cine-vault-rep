using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Entities;
using Mapster;

namespace CineVault.API.Mappers;

public sealed class ActorProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ActorRequest, Actor>()
            .MapWith(a => new Actor(a.FullName, a.BirthDate, a.Biography));

        config.NewConfig<Actor, ActorResponse>();
    }
}