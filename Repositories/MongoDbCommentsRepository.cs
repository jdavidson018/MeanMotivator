using System;
using System.Collections.Generic;
using MeanMotivator.Models;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace MeanMotivator.Repositories{
    public class MongoDbCommentsRepository : ICommentsRepository
    {
        private const string databaseName = "catalog";
        private const string collectionName = "comments";
        private readonly IMongoCollection<Comment> commentsCollection;
        private readonly FilterDefinitionBuilder<Comment> filterBuilder = Builders<Comment>.Filter;
        public MongoDbCommentsRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            commentsCollection = database.GetCollection<Comment>(collectionName);
        }
        public async Task CreateCommentAsync(Comment comment)
        {
            await commentsCollection.InsertOneAsync(comment);
        }

        public async Task DeleteCommentAsync(Guid id)
        {
            var filter = filterBuilder.Eq(comment => comment.Id, id);
            await commentsCollection.DeleteOneAsync(filter);
        }

        public async Task<Comment> GetCommentAsync(Guid id)
        {
            var filter = filterBuilder.Eq(comment => comment.Id, id);
            return await commentsCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Comment>> GetCommentsAsync()
        {
            return await commentsCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task UpdateCommentAsync(Comment comment)
        {
            var filter = filterBuilder.Eq(existingComment => existingComment.Id, comment.Id);
            await commentsCollection.ReplaceOneAsync(filter, comment);
        }

        public async Task<Comment> GetRandomCommentAsync()
        {
            var allComments = await commentsCollection.Find(new BsonDocument()).ToListAsync();
            var random = new Random();
            int index = random.Next(allComments.Count);
            return await Task.FromResult(allComments[index]);
        }
    }
}