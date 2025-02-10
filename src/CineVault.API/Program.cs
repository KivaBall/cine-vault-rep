using CineVault.API.Extensions;
using CineVault.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext(builder.Configuration);
builder.Services.AddVersioning();
builder.Services.AddEndpoints();
builder.Services.AddLoggingWithSerilog(builder.Configuration);
builder.Services.AddSwagger(builder.Environment);
builder.Services.AddMappers();
builder.Services.AddCaching(builder.Configuration);

var app = builder.Build();

app.ConfigureDb();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerMiddleware();
}

if (app.Environment.IsLocal())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<SerilogMiddleware>();
app.MapControllers();

Console.WriteLine($"Current environment is {app.Environment.EnvironmentName}");

await app.RunAsync();