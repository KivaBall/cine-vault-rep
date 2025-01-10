namespace CineVault.API.Extensions;

public static class EnvExtensions
{
    public static bool IsLocal(this IHostEnvironment environment)
    {
        return environment.EnvironmentName == "Local";
    }
}