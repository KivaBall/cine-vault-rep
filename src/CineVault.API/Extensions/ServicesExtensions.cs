namespace CineVault.API.Extensions;

public static class ServicesExtensions
{
    public static void ConfigureServices(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddDbContext(configuration);

        services.AddEndpoints();

        services.AddLoggingWithSerilog(configuration);

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

    private static void AddEndpoints(this IServiceCollection services)
    {
        services.AddRouting(opt => opt.LowercaseUrls = true);

        services.AddControllers();

        services.AddEndpointsApiExplorer();
    }

    private static void AddLoggingWithSerilog(this IServiceCollection services,
        IConfiguration configuration)
    {
        var logLevelStr = configuration["Logging:LogLevel:Default"];

        if (logLevelStr == null)
        {
            throw new InvalidOperationException("Logging level is not configured");
        }

        var isLogLevel = Enum.TryParse<LogEventLevel>(logLevelStr, out var logLevel);

        if (!isLogLevel)
        {
            throw new InvalidOperationException("Logging level is not correct");
        }

        services.AddSerilog(config =>
        {
            config
                .MinimumLevel.Is(logLevel)
                .WriteTo.Console()
                .WriteTo.File("logging.txt",
                    outputTemplate:
                    "{Timestamp:dd}.{Timestamp:MM}, {Timestamp:HH:mm:ss} => {Level:u3} | {Message:lj}{NewLine}");
        });

        services.AddScoped<SerilogMiddleware>();
    }

    private static void AddSwagger(this IServiceCollection services,
        IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            services.AddSwaggerGen();
        }
    }
}