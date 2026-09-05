using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Infrastructure.Mongo
{
    public interface IMongoIntegration
    {
        Task<T?> GetAsync<T>(
            string collectionName,
            FilterDefinition<T> filter);

        Task<List<T>> GetAllAsync<T>(
            string collectionName);

        Task<List<T>> GetManyAsync<T>(
            string collectionName,
            FilterDefinition<T> filter);

        Task InsertAsync<T>(
            string collectionName,
            T entity);

        Task InsertManyAsync<T>(
            string collectionName,
            IEnumerable<T> entities);

        Task<bool> UpdateAsync<T>(
            string collectionName,
            FilterDefinition<T> filter,
            UpdateDefinition<T> update);

        Task<bool> DeleteAsync<T>(
            string collectionName,
            FilterDefinition<T> filter);
    }
}
