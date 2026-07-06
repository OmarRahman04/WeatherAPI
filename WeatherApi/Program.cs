using Amazon.Lambda.AspNetCoreServer.Hosting;
using WeatherApi.Configuration;
using WeatherApi.Services;

var builder = WebApplication.CreateBuilder(args);

// MongoDB Configuration
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<MongoDbService>();

// Register Weather Service
builder.Services.AddScoped<IWeatherService, WeatherService>();

// If WeatherService uses HttpClient, uncomment this line:
// builder.Services.AddHttpClient();

// Lambda Hosting
builder.Services.AddAWSLambdaHosting(
    LambdaEventSource.HttpApi
);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

app.MapControllers();

// ADD THIS HERE
foreach (var endpoint in app.Services
         .GetRequiredService<Microsoft.AspNetCore.Routing.EndpointDataSource>()
         .Endpoints)
{
    Console.WriteLine(endpoint.DisplayName);
}
app.MapGet("/test", () => "Lambda Working");
app.Run();