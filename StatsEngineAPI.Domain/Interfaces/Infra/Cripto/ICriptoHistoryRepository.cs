using StatsEngineAPI.Worker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Domain.Interfaces.Infra.Cripto
{
    public interface ICriptoHistoryRepository
    {
        Task<CriptoResponseModel?> GetAsync(string symbol);
        Task<List<CriptoResponseModel>> GetAllAsync();
        Task InsertAsync(CriptoResponseModel obj);
        Task InsertManyAsync(IEnumerable<CriptoResponseModel> objs);
        Task<bool> UpdateAsync(string symbol, decimal price);
        Task<bool> UpdateAsync(string symbol, decimal price, DateTime insert);
        Task<bool> ReplaceAsync(CriptoResponseModel obj);
        Task<bool> DeleteAsync(string symbol);

    }
}
