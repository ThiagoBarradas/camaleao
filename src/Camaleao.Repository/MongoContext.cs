using MongoDB.Driver;

namespace Camaleao.Repository
{
    public class MongoContext
    {
        private readonly IMongoDatabase _database;

        public MongoContext(Settings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.Database);
        }

        public IMongoCollection<T> GetCollection<T>(string collection)
        {
            return _database.GetCollection<T>(collection);
        }
    }
}
