using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

using InclusingLenguage._01_Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InclusingLenguage._04_Services
{
    public interface IMongoDBService
    {
        IMongoDatabase GetDatabase();
        IMongoCollection<T> GetCollection<T>(string collectionName);
        Task<bool> TestConnectionAsync();
    }

    public class MongoDBService : IMongoDBService
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;

        // Cadena de conexión - CAMBIAR ESTO CON TUS CREDENCIALES
        private const string ConnectionString = "mongodb+srv://Camila:k8ywehVMaGlPvIC3@cluster0.mbrrbur.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";
        private const string DatabaseName = "signlearn_db";

        public MongoDBService()
        {
            try
            {
                var settings = MongoClientSettings.FromConnectionString(ConnectionString);
                settings.ServerApi = new ServerApi(ServerApiVersion.V1);
                _client = new MongoClient(settings);
                _database = _client.GetDatabase(DatabaseName);

                // Crear índices si no existen
                CreateIndexes();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error conectando a MongoDB: {ex.Message}");
                throw;
            }
        }

        public IMongoDatabase GetDatabase()
        {
            return _database;
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                var adminDb = _client.GetDatabase("admin");
                var command = new BsonDocument("ping", 1);
                await adminDb.RunCommandAsync<BsonDocument>(command);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en test de conexión: {ex.Message}");
                return false;
            }
        }

        private void CreateIndexes()
        {
            try
            {
                // Índice en usuarios por email (único)
                var usersCollection = _database.GetCollection<BsonDocument>("users");
                var emailIndexModel = new CreateIndexModel<BsonDocument>(
                    Builders<BsonDocument>.IndexKeys.Ascending("email"),
                    new CreateIndexOptions { Unique = true });
                usersCollection.Indexes.CreateOne(emailIndexModel);

                // Índice en lesson_records por userId y lessonId
                var lessonsCollection = _database.GetCollection<BsonDocument>("lesson_records");
                var lessonIndexModel = new CreateIndexModel<BsonDocument>(
                    Builders<BsonDocument>.IndexKeys.Ascending("userId").Ascending("lessonId"));
                lessonsCollection.Indexes.CreateOne(lessonIndexModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creando índices: {ex.Message}");
            }
        }
    }

    

   

    

    
}
