using System.Linq.Expressions;
using MongoDB.Driver;

namespace StatsEngineAPI.Infrastructure.Mongo
{
    public interface IMongoDbIntegration
    {
        Task<T?> GetAsync<T>(
            string collectionName,
            Expression<Func<T, bool>> filter);

        Task<List<T>> GetAllAsync<T>(string collectionName);

        Task<List<T>> GetManyAsync<T>(
            string collectionName,
            Expression<Func<T, bool>> filter);

        Task InsertAsync<T>(string collectionName, T entity);

        Task InsertManyAsync<T>(string collectionName, IEnumerable<T> entities);

        // Substitui o documento inteiro
        Task<bool> ReplaceAsync<T>(
            string collectionName,
            Expression<Func<T, bool>> filter,
            T entity);

        // Atualiza apenas os campos informados (personalizável por repositório)
        Task<bool> UpdateAsync<T>(
            string collectionName,
            Expression<Func<T, bool>> filter,
            params (Expression<Func<T, object>> Field, object Value)[] updates);

        Task<bool> DeleteAsync<T>(
            string collectionName,
            Expression<Func<T, bool>> filter);
    }
}