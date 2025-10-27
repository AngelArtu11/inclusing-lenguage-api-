using MongoDB.Driver;
using InclusingLenguage.API.Models;
using Microsoft.Extensions.Options;

namespace InclusingLenguage.API.Services
{
    public interface IMongoDBService
    {
        IMongoCollection<UserProfile> Users { get; }
        IMongoCollection<Lesson> Lessons { get; }
        Task<bool> TestConnectionAsync();
    }

    public class MongoDBService : IMongoDBService
    {
        private readonly IMongoDatabase _database;

        public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            var settings = MongoClientSettings.FromConnectionString(mongoDBSettings.Value.ConnectionString);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);

            var client = new MongoClient(settings);
            _database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        }

        public IMongoCollection<UserProfile> Users =>
            _database.GetCollection<UserProfile>("users");

        public IMongoCollection<Lesson> Lessons =>
            _database.GetCollection<Lesson>("lessons");

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                await _database.RunCommandAsync<MongoDB.Bson.BsonDocument>(new MongoDB.Bson.BsonDocument("ping", 1));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
