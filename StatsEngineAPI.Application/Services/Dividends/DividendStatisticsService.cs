using StatsEngineAPI.Application.Helpers;
using StatsEngineAPI.Domain.Dividends;
using StatsEngineAPI.Domain.Interfaces.Services.Dividends;
using StatsEngineAPI.Infrastructure.Repository.Nasdaq;


namespace StatsEngineAPI.Application.Services.Dividends;

public class DividendStatisticsService : IDividendStatsticService
{

    private readonly CalcYieldService _calcYieldService;
    private readonly CalcGrowthService _calcGrowth;

    private readonly ParseDataHelper _parseDataHelper;
    private readonly ParseDecimalHelper _parseDecimalHelper;

    private readonly AlphaDividendsConsumer _alphaDividendsConsumer;
    public DividendStatisticsService(ParseDecimalHelper parseDecimalHelper, 
        ParseDataHelper parseDataHelper,
        CalcYieldService calcYieldService,
        CalcGrowthService calcGrowth,
        AlphaDividendsConsumer alphaDividendsConsumer
        )
    {
        _parseDecimalHelper = parseDecimalHelper;
        _parseDataHelper = parseDataHelper;
        _calcYieldService = calcYieldService;
        _calcGrowth = calcGrowth;
        _alphaDividendsConsumer = alphaDividendsConsumer;
    }
    public DividendStatisticsDto CalcularEstatisticas(
        string symbol,
        List<DividendEntry> dividendos,
        decimal precoAtual)
    {
        return new DividendStatisticsDto
        {
            Symbol = symbol,
            YieldAnualizado = _calcYieldService.CalcularYield(dividendos, precoAtual),
            CrescimentoDividendos = _calcGrowth.CalcularCrescimento(dividendos),
            Consistencia = CalcularConsistencia(dividendos),
            CalculadoEm = DateTime.UtcNow
        };
    }


    // ── CONSISTÊNCIA DE PAGAMENTO ──────────────────────────────────────

    public DividendConsistencyDto CalcularConsistencia(
        List<DividendEntry> dividendos,
        int diasGapAlerta = 90)
    {
        var ordenados = dividendos.Where(d => !string.IsNullOrWhiteSpace(d.PaymentDate))
           .OrderBy(d => (d.PaymentDate))
           .ToList();
        var total = ordenados.Count;

        if (total == 0)
        {
            return new DividendConsistencyDto
            {
                TotalPagamentos = 0,
                DiasNoPeriodo = 0,
                MediaDiasEntrePagamentos = 0,
                ConsistenciaPercentual = 0,
                Classificacao = "Sem dados",
                AlertaGap = "Nenhum pagamento encontrado no período."
            };
        }

        var datas = ordenados.Select(d => _parseDataHelper.ParseData(d.PaymentDate)).ToList();
        var diasPeriodo = (int)(datas.Last() - datas.First()).TotalDays;

        // Média de dias entre cada pagamento
        double mediaEntrePagamentos = 0;
        string? alertaGap = null;
        int maiorGap = 0;

        if (total > 1)
        {
            var intervalos = new List<int>();
            for (var i = 1; i < datas.Count; i++)
            {
                var gap = (int)(datas[i] - datas[i - 1]).TotalDays;
                intervalos.Add(gap);

                if (gap > maiorGap) maiorGap = gap;
            }

            mediaEntrePagamentos = Math.Round(intervalos.Average(), 1);

            if (maiorGap > diasGapAlerta)
                alertaGap = $"Gap de {maiorGap} dias detectado — acima do limite de {diasGapAlerta} dias.";
        }

        // Pagamentos esperados = períodos completos de ~30 dias
        var pagamentosEsperados = diasPeriodo > 0
            ? Math.Max(1, (int)Math.Round(diasPeriodo / 30.0))
            : total;

        var consistencia = Math.Min(100, Math.Round((decimal)total / pagamentosEsperados * 100, 1));

        return new DividendConsistencyDto
        {
            TotalPagamentos = total,
            DiasNoPeriodo = diasPeriodo,
            MediaDiasEntrePagamentos = mediaEntrePagamentos,
            ConsistenciaPercentual = consistencia,
            Classificacao = ClassificarConsistencia(consistencia),
            AlertaGap = alertaGap
        };
    }

    private static string ClassificarConsistencia(decimal consistencia) => consistencia switch
    {
        >= 90 => "Muito Regular",
        >= 60 => "Regular",
        _ => "Irregular"
    };
}
