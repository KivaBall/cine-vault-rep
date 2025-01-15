namespace CineVault.API.Extensions;

public static class ServicesExtensions
{
    public static void ConfigureServices(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddDbContext(configuration);

        services.AddEndpoints();

        services.AddSwagger(environment);

        services.AddMappers();
    }

    private static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CineVaultDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("CineVaultDb");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string is not configured");
            }

            options.UseInMemoryDatabase(connectionString);
        });
    }

    private static void AddEndpoints(this IServiceCollection services)
    {
        services.AddRouting(opt => opt.LowercaseUrls = true);

        services.AddControllers();

        services.AddEndpointsApiExplorer();
    }

    private static void AddSwagger(this IServiceCollection services,
        IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            services.AddSwaggerGen();
        }
    }

    private static void AddMappers(this IServiceCollection services)
    {
        services.AddMapster();
        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
    }
}