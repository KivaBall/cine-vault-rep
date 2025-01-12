namespace CineVault.API.Extensions;

public static class ServicesExtensions
{
    public static void ConfigureServices(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddDbContext(configuration);

        services.AddVersioning();

        services.AddEndpoints();

        services.AddSwagger(environment);
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

    private static void AddVersioning(this IServiceCollection services)
    {
        services
            .AddApiVersioning(opt =>
            {
                opt.DefaultApiVersion = new ApiVersion(1);
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddMvc()
            .AddApiExplorer(opt =>
            {
                opt.GroupNameFormat = "'v'V";
                opt.SubstituteApiVersionInUrl = true;
            });
    }

    private static void AddEndpoints(this IServiceCollection services)
    {
        services.AddRouting(opt => opt.LowercaseUrls = true);

        services.AddControllers();

        services.AddEndpointsApiExplorer();
    }

    private static void AddSwagger(this IServiceCollection services, IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "1",
                    Title = "CineVault API"
                });
                opt.SwaggerDoc("v2", new OpenApiInfo
                {
                    Version = "2",
                    Title = "CineVault API"
                });
            });
        }
    }
}