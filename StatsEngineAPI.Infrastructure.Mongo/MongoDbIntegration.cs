using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace StatsEngineAPI.Infrastructure.Mongo
{
    public class MongoDbIntegration : IMongoDbIntegration
    {
        private readonly IMongoDatabase _database;
        private readonly ILogger<MongoDbIntegration> _logger;
        private const string ClassName = nameof(MongoDbIntegration);

        public MongoDbIntegration(
            IMongoDatabase database,
            ILogger<MongoDbIntegration> logger)
        {
            _database = database;
            _logger = logger;
        }

        private IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }

        public async Task<T?> GetAsync<T>(
            string collectionName,
            Expression<Func<T, bool>> filter)
        {
            try
            {
                var collection = GetCollection<T>(collectionName);
                return await collection.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[{Class}] [{Method}] Erro ao consultar a collection {CollectionName}",
                    ClassName,
                    nameof(GetAsync),
                    collectionName);
                throw;
            }
        }

        public async Task<List<T>> GetAllAsync<T>(string collectionName)
        {
            try
            {
                var collection = GetCollection<T>(collectionName);
                return await collection.Find(FilterDefinition<T>.Empty).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[{Class}] [{Method}] Erro ao consultar todos os documentos da collection {CollectionName}",
                    ClassName,
                    nameof(GetAllAsync),
                    collectionName);
                throw;
            }
        }

        public async Task<List<T>> GetManyAsync<T>(
            string collectionName,
            Expression<Func<T, bool>> filter)
        {
            try
            {
                var collection = GetCollection<T>(collectionName);
                return await collection.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[{Class}] [{Method}] Erro ao consultar múltiplos documentos da collection {CollectionName}",
                    ClassName,
                    nameof(GetManyAsync),
                    collectionName);
                throw;
            }
        }

        public async Task InsertAsync<T>(string collectionName, T entity)
        {
            try
            {
                var collection = GetCollection<T>(collectionName);
                await collection.InsertOneAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[{Class}] [{Method}] Erro ao inserir documento na collection {CollectionName}",
                    ClassName,
                    nameof(InsertAsync),
                    collectionName);
                throw;
            }
        }

        public async Task InsertManyAsync<T>(string collectionName, IEnumerable<T> entities)
        {
            try
            {
                var collection = GetCollection<T>(collectionName);
                await collection.InsertManyAsync(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[{Class}] [{Method}] Erro ao inserir múltiplos documentos na collection {CollectionName}",
                    ClassName,
                    nameof(InsertManyAsync),
                    collectionName);
                throw;
            }
        }

        public async Task<bool> ReplaceAsync<T>(
            string collectionName,
            Expression<Func<T, bool>> filter,
            T entity)
        {
            try
            {
                var collection = GetCollection<T>(collectionName);
                var result = await collection.ReplaceOneAsync(filter, entity);
                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[{Class}] [{Method}] Erro ao substituir documento na collection {CollectionName}",
                    ClassName,
                    nameof(ReplaceAsync),
                    collectionName);
                throw;
            }
        }

        public async Task<bool> UpdateAsync<T>(
            string collectionName,
            Expression<Func<T, bool>> filter,
            params (Expression<Func<T, object>> Field, object Value)[] updates)
        {
            try
            {
                if (updates is null || updates.Length == 0)
                    throw new ArgumentException("Informe ao menos um campo para atualizar.", nameof(updates));

                var collection = GetCollection<T>(collectionName);

                var updateDefinitions = updates.Select(u =>
                    Builders<T>.Update.Set(u.Field, u.Value));

                var combinedUpdate = Builders<T>.Update.Combine(updateDefinitions);

                var result = await collection.UpdateOneAsync(filter, combinedUpdate);
                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[{Class}] [{Method}] Erro ao atualizar documento na collection {CollectionName}",
                    ClassName,
                    nameof(UpdateAsync),
                    collectionName);
                throw;
            }
        }

        public async Task<bool> DeleteAsync<T>(
            string collectionName,
            Expression<Func<T, bool>> filter)
        {
            try
            {
                var collection = GetCollection<T>(collectionName);
                var result = await collection.DeleteOneAsync(filter);
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[{Class}] [{Method}] Erro ao deletar documento na collection {CollectionName}",
                    ClassName,
                    nameof(DeleteAsync),
                    collectionName);
                throw;
            }
        }
    }
}