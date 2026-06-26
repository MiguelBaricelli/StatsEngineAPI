using StatsEngineAPI.Domain.Dividends;
using StatsEngineAPI.Infrastructure.Repository.Nasdaq;
using StatsEngineAPI.Application.Services.Dividends;

namespace StatsEngineAPI.Application.Services;

/// <summary>
/// Orquestra a busca de dados e geração de estatísticas completas.
/// Não contém mais lógica de cálculo duplicada — delega para os services especializados.
/// </summary>
public class GenerateStatsService
{
    
    private readonly DividendStatisticsService _dividendStatisticsService;
    private readonly MarketDataCentralizerIntegration _marketDataCentralizerIntegration;

    public GenerateStatsService(
        DividendStatisticsService dividendStatisticsService,
        MarketDataCentralizerIntegration marketDataCentralizerIntegration)
    {
        _dividendStatisticsService = dividendStatisticsService;
        _marketDataCentralizerIntegration = marketDataCentralizerIntegration;
    }

    public async Task<DividendStatsResultDto> GenerateStats(string ticker, decimal precoAtual, decimal? precoMedioCompra = null)
    {
        try
        {
            var dividendData = await _marketDataCentralizerIntegration.GetDividendsData(ticker);

            if (dividendData?.Data == null || !dividendData.Data.Any())
            {
                return new DividendStatsResultDto
                {
                    Estatisticas = null,
                    GrowthEntries = new List<DividendGrowthEntryDto>()
                };
            }

            var dividendEntries = dividendData.Data
                .Select(d => new DividendEntry
                {
                    PaymentDate = d.PaymentDate,
                    Amount = d.Amount
                })
                .ToList();

            var estatisticas = _dividendStatisticsService.CalcularEstatisticas(
                ticker,
                dividendEntries,
                precoAtual,
                precoMedioCompra
            );

            // GrowthEntries mantidos separados para uso em gráficos de linha no dashboard
            var growthEntries = estatisticas.Crescimento.Entradas;

            return new DividendStatsResultDto
            {
                Estatisticas = estatisticas,
                GrowthEntries = growthEntries
            }; 
        } catch (Exception e)
        {
            throw new Exception(e.Message + " | Erro na geração das estatisticas | ");
        }
         
    }

    public class DividendStatsResultDto
    {
        public DividendStatisticsDto? Estatisticas { get; set; }
        public List<DividendGrowthEntryDto> GrowthEntries { get; set; } = new();
    }
}
