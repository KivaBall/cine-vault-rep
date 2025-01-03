namespace CineVault.API.Extensions;

public static class EnvExtensions
{
    public static bool IsLocal(this IConfiguration configuration)
    {
        var environment = configuration["ASPNETCORE_ENVIRONMENT"];
        
        if (environment == null)
        {
            throw new Exception("Something went wrong");
        }

        return environment == "Local";
    }
}