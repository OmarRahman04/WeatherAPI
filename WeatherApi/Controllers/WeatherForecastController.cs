using Microsoft.AspNetCore.Mvc;
using WeatherApi.Models;
using WeatherApi.Services;

namespace WeatherApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IWeatherService _weatherService;
    private readonly MongoDbService _mongoDbService;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        IWeatherService weatherService,
        MongoDbService mongoDbService)
    {
        _logger = logger;
        _weatherService = weatherService;
        _mongoDbService = mongoDbService;
    }

    // 👇 ADD IT HERE
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok("Hello from GitHub CI/CD Pipeline");
    }

    [HttpGet("search")]
    public IEnumerable<SearchLocation> GetSearchLocations(string? q, string? lang = "en")
    {
        if (string.IsNullOrEmpty(q))
        {
            return [];
        }

        return _weatherService.FetchLocation(query: q, lang: lang!);
    }

    [HttpGet("forecast")]
    public async Task<Weather> GetWeatherForecasts(string? q, int days = 3, string? lang = "en")
    {
        if (string.IsNullOrEmpty(q))
        {
            return new Weather();
        }

        var weather = _weatherService.FetchWeatherForecast(query: q, days: days, lang: lang!);

        var record = new WeatherRecord
        {
            City = weather.location?.name ?? "",
            Country = weather.location?.country ?? "",
            Temperature = weather.current?.temp_c ?? 0,
            Humidity = weather.current?.humidity ?? 0,
            Condition = weather.current?.condition?.text ?? ""
        };

        await _mongoDbService.SaveWeatherAsync(record);

        return weather;
    }
}