using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using StatsEngineAPI.Domain.Interfaces.Infra.Cripto;
using StatsEngineAPI.Infrastructure.Mongo;
using StatsEngineAPI.Worker.Models;

public class CriptoHistoryRepository : ICriptoHistoryRepository
{
    private readonly IMongoDbIntegration _mongoDbIntegration;

    private readonly ILogger<CriptoHistoryRepository> _logger;

    private const string CollectionName = "CriptoHistory";

    private const string ClassName = nameof(CriptoHistoryRepository);

    public CriptoHistoryRepository(
        IMongoDbIntegration mongoDbIntegration,
        ILogger<CriptoHistoryRepository> logger)
    {
        _mongoDbIntegration = mongoDbIntegration;
        _logger = logger;
    }

    public async Task<CriptoResponseModel?> GetAsync(string symbol)
    {
        try
        {
            return await _mongoDbIntegration.GetAsync<CriptoResponseModel>(
                CollectionName,
                x => x.Name == symbol);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[{Class}] [{Method}] Erro ao consultar cripto {Symbol}",
                ClassName,
                nameof(GetAsync),
                symbol);
            throw;
        }
    }

    public async Task<List<CriptoResponseModel>> GetAllAsync()
    {
        try
        {
            return await _mongoDbIntegration.GetAllAsync<CriptoResponseModel>(CollectionName);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[{Class}] [{Method}] Erro ao consultar todas as criptos",
                ClassName,
                nameof(GetAllAsync));
            throw;
        }
    }

    public async Task InsertAsync(CriptoResponseModel obj)
    {
        try
        {
            await _mongoDbIntegration.InsertAsync(CollectionName, obj);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[{Class}] [{Method}] Erro ao inserir cripto {Symbol}",
                ClassName,
                nameof(InsertAsync),
                obj?.Name);
            throw;
        }
    }

    public async Task InsertManyAsync(IEnumerable<CriptoResponseModel> objs)
    {
        try
        {
            await _mongoDbIntegration.InsertManyAsync(CollectionName, objs);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[{Class}] [{Method}] Erro ao inserir múltiplas criptos",
                ClassName,
                nameof(InsertManyAsync));
            throw;
        }
    }

    // Atualiza só o preço, sem tocar no resto do documento
    public async Task<bool> UpdateAsync(string symbol, decimal price)
    {
        try
        {
            return await _mongoDbIntegration.UpdateAsync<CriptoResponseModel>(
                CollectionName,
                x => x.Name == symbol,
                (x => x.Price, price));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[{Class}] [{Method}] Erro ao atualizar preço da cripto {Symbol}",
                ClassName,
                nameof(UpdateAsync),
                symbol);
            throw;
        }
    }

    // Exemplo: atualizar mais de um campo de forma personalizada
    public async Task<bool> UpdateAsync(string symbol, decimal price, DateTime insert)
    {
        try
        {
            return await _mongoDbIntegration.UpdateAsync<CriptoResponseModel>(
                CollectionName,
                x => x.Name == symbol,
                (x => x.Price, price),
                (x => x.DateInsert, insert));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[{Class}] [{Method}] Erro ao atualizar preço e data da cripto {Symbol}",
                ClassName,
                nameof(UpdateAsync),
                symbol);
            throw;
        }
    }

    // Substitui o objeto inteiro
    public async Task<bool> ReplaceAsync(CriptoResponseModel obj)
    {
        try
        {
            return await _mongoDbIntegration.ReplaceAsync(
                CollectionName,
                x => x.Name == obj.Name,
                obj);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[{Class}] [{Method}] Erro ao substituir cripto {Symbol}",
                ClassName,
                nameof(ReplaceAsync),
                obj?.Name);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string symbol)
    {
        try
        {
            return await _mongoDbIntegration.DeleteAsync<CriptoResponseModel>(
                CollectionName,
                x => x.Name == symbol);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[{Class}] [{Method}] Erro ao deletar cripto {Symbol}",
                ClassName,
                nameof(DeleteAsync),
                symbol);
            throw;
        }
    }
}