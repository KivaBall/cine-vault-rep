var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureServices(builder.Configuration, builder.Environment);

var app = builder.Build();

app.ConfigureDb();

app.BuildPipeline();

await app.RunAsync();