using VigilantMeerkat.Micro.Base.Config;
using MongoDB.Driver;
using VigilantMeerkat.Micro.Base.Vault;

namespace VigilantMeerkat.Micro.Base.Logger
{
    public class MongoConnector
    {
        private readonly MongoConfig _configDefinition;
        private readonly MongoClient _mongoClient;

        public MongoConnector(MongoConfig configDefinition)
        {
            _configDefinition = configDefinition;
            _mongoClient = new MongoClient(configDefinition.ConnectionString);
        }

        public void Insert<T>(T entry, string name)
        {
            _mongoClient.GetDatabase("log").GetCollection<T>(name).InsertOne(entry);
        }
    }
}