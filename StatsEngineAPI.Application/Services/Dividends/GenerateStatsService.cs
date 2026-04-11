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
    private readonly AlphaDividendsConsumer _alphaDividendsConsumer;
    private readonly DividendStatisticsService _dividendStatisticsService;

    public GenerateStatsService(
        AlphaDividendsConsumer alphaDividendsConsumer,
        DividendStatisticsService dividendStatisticsService)
    {
        _alphaDividendsConsumer = alphaDividendsConsumer;
        _dividendStatisticsService = dividendStatisticsService;
    }

    public async Task<DividendStatsResultDto> GenerateStats(string ticker, decimal precoAtual, decimal? precoMedioCompra = null)
    {
        var dividendData = await _alphaDividendsConsumer.AlphaDividensConsumer(ticker);

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
    }

    public class DividendStatsResultDto
    {
        public DividendStatisticsDto? Estatisticas { get; set; }
        public List<DividendGrowthEntryDto> GrowthEntries { get; set; } = new();
    }
}
