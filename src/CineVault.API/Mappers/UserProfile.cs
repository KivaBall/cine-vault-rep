namespace CineVault.API.Mappers;

public sealed class UserProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserRequest, User>()
            .MapWith(u => new User(u.Username, u.Email, u.Password));

        config.NewConfig<User, UserResponse>();
    }
}