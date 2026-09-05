using MongoDB.Driver;

namespace StatsEngineAPI.Infrastructure.Mongo
{
    public class MongoIntegration : IMongoIntegration
    {
        private readonly IMongoDatabase _database;

        public MongoIntegration(IMongoDatabase database)
        {
            _database = database;
        }

        private IMongoCollection<T> GetCollection<T>(
            string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }

        public async Task<T?> GetAsync<T>(
            string collectionName,
            FilterDefinition<T> filter)
        {
            var collection = GetCollection<T>(collectionName);

            return await collection
                .Find(filter)
                .FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAllAsync<T>(
            string collectionName)
        {
            var collection = GetCollection<T>(collectionName);

            return await collection
                .Find(FilterDefinition<T>.Empty)
                .ToListAsync();
        }

        public async Task<List<T>> GetManyAsync<T>(
            string collectionName,
            FilterDefinition<T> filter)
        {
            var collection = GetCollection<T>(collectionName);

            return await collection
                .Find(filter)
                .ToListAsync();
        }

        public async Task InsertAsync<T>(
            string collectionName,
            T entity)
        {
            var collection = GetCollection<T>(collectionName);

            await collection.InsertOneAsync(entity);
        }

        public async Task InsertManyAsync<T>(
            string collectionName,
            IEnumerable<T> entities)
        {
            var collection = GetCollection<T>(collectionName);

            await collection.InsertManyAsync(entities);
        }

        public async Task<bool> UpdateAsync<T>(
            string collectionName,
            FilterDefinition<T> filter,
            UpdateDefinition<T> update)
        {
            var collection = GetCollection<T>(collectionName);

            var result = await collection.UpdateOneAsync(
                filter,
                update);

            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync<T>(
            string collectionName,
            FilterDefinition<T> filter)
        {
            var collection = GetCollection<T>(collectionName);

            var result = await collection.DeleteOneAsync(filter);

            return result.DeletedCount > 0;
        }
    }
}
