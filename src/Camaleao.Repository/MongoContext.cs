
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Repository
{
    public class MongoContext
    {
        private readonly IMongoDatabase _database;

        public MongoContext(Settings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(settings.Database);
        }

        public IMongoCollection<T> GetCollection<T>(string collection)
        {
           
            return _database.GetCollection<T>(collection);
        }
    }
}
