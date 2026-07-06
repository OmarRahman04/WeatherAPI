using Amazon.Lambda.AspNetCoreServer.Hosting;
using Microsoft.AspNetCore.Routing;
using WeatherApi.Configuration;
using WeatherApi.Services;

var builder = WebApplication.CreateBuilder(args);

// MongoDB Configuration
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<MongoDbService>();

// Register Weather Service
builder.Services.AddScoped<IWeatherService, WeatherService>();

// Lambda Hosting
builder.Services.AddAWSLambdaHosting(
    LambdaEventSource.HttpApi);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
app.UseSwagger(c =>
{
    c.RouteTemplate = "weather/swagger/{documentName}/swagger.json";
});

app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "weather/swagger";
    c.SwaggerEndpoint("/weather/swagger/v1/swagger.json", "WeatherApi API V1");
});

app.UseHttpsRedirection();

app.UseAuthorization();

// Map Controllers
app.MapControllers();

// Simple test endpoint
app.MapGet("/test", () => "Lambda Working");

// Print all registered endpoints
var endpointDataSource = app.Services.GetRequiredService<EndpointDataSource>();

Console.WriteLine("========== REGISTERED ENDPOINTS ==========");

foreach (var endpoint in endpointDataSource.Endpoints)
{
    Console.WriteLine(endpoint.DisplayName);
}

Console.WriteLine("==========================================");

app.Run();