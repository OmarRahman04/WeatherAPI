using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WeatherApi.Configuration;
using WeatherApi.Models;

namespace WeatherApi.Services
{
    public class MongoDbService
    {
        private readonly IMongoCollection<WeatherRecord> _weatherCollection;

        public MongoDbService(IOptions<MongoDbSettings> settings)
        {
            var mongoClient = new MongoClient(settings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);

            _weatherCollection = mongoDatabase.GetCollection<WeatherRecord>(
       settings.Value.CollectionName);
        }

        public async Task SaveWeatherAsync(WeatherRecord weather)
        {
            await _weatherCollection.InsertOneAsync(weather);
        }

        public async Task<List<WeatherRecord>> GetWeatherHistoryAsync()
        {
            return await _weatherCollection
                .Find(_ => true)
                .SortByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
    }
}