namespace CineVault.API.Mappers;

public sealed class UserProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserRequest, User>();

        config.NewConfig<User, UserResponse>();
    }
}