using StatsEngineAPI.Domain.Dividends;
using StatsEngineAPI.Domain.Interfaces.Services.Dividends;
using StatsEngineAPI.Infrastructure.Repository.Nasdaq;

namespace StatsEngineAPI.Application.Services.Dividends;

/// <summary>
/// Orquestra os serviços de cálculo e retorna o DTO consolidado de estatísticas.
/// Não contém lógica de cálculo — delega para os services especializados.
/// </summary>
public class DividendStatisticsService : IDividendStatsticService
{
    private readonly CalcYieldService _calcYieldService;
    private readonly CalcGrowthService _calcGrowthService;
    private readonly CalcConsistencyService _calcConsistencyService;
    private readonly AlphaDividendsConsumer _alphaDividendsConsumer;

    public DividendStatisticsService(
        CalcYieldService calcYieldService,
        CalcGrowthService calcGrowthService,
        CalcConsistencyService calcConsistencyService,
        AlphaDividendsConsumer alphaDividendsConsumer)
    {
        _calcYieldService = calcYieldService;
        _calcGrowthService = calcGrowthService;
        _calcConsistencyService = calcConsistencyService;
        _alphaDividendsConsumer = alphaDividendsConsumer;
    }

    public DividendStatisticsDto CalcularEstatisticas(
        string symbol,
        List<DividendEntry> dividendos,
        decimal precoAtual,
        decimal? precoMedioCompra = null)
    {
        return new DividendStatisticsDto
        {
            Symbol = symbol,
            Yield = _calcYieldService.CalcularYield(dividendos, precoAtual, precoMedioCompra),
            Crescimento = _calcGrowthService.CalcularCrescimento(dividendos),
            Consistencia = _calcConsistencyService.CalcularConsistencia(dividendos),
            CalculadoEm = DateTime.UtcNow
        };
    }
}