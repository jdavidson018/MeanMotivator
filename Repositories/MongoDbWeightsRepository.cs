using System;
using System.Collections.Generic;
using MeanMotivator.Models;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace MeanMotivator.Repositories{
    public class MongoDbWeightsRepository : IWeightsRepository
    {
        private const string databaseName = "catalog";
        private const string collectionName = "weights";
        private readonly IMongoCollection<Weight> weightsCollection;
        private readonly FilterDefinitionBuilder<Weight> filterBuilder = Builders<Weight>.Filter;
        public MongoDbWeightsRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            weightsCollection = database.GetCollection<Weight>(collectionName);
        }

        public async Task CreateWeightAsync(Weight weight)
        {
            await weightsCollection.InsertOneAsync(weight);
        }

        public async Task DeleteWeightAsync(Guid id)
        {
            var filter = filterBuilder.Eq(weight => weight.Id, id);
            await weightsCollection.DeleteOneAsync(filter);
        }

        public async Task<Weight> GetWeightAsync(Guid id)
        {
            var filter = filterBuilder.Eq(weight => weight.Id, id);
            return await weightsCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Weight>> GetWeightsAsync()
        {
            return await weightsCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task UpdateWeightAsync(Weight weight)
        {
            var filter = filterBuilder.Eq(existingWeight => existingWeight.Id, weight.Id);
            await weightsCollection.ReplaceOneAsync(filter, weight);
        }
    }
}