using StatsEngineAPI.Application.Helpers;
using StatsEngineAPI.Application.Services.Dividends;
using StatsEngineAPI.Domain.Dividends;
using StatsEngineAPI.Infrastructure.Repository.Nasdaq;

public class GenerateStatsService
{
    private readonly AlphaDividendsConsumer _alphaDividendsConsumer;
    private readonly CalcYieldService _calcYieldService;
    private readonly CalcGrowthService _calcGrowth;

    private readonly ParseDataHelper _parseDataHelper;
    private readonly ParseDecimalHelper _parseDecimalHelper;

    public GenerateStatsService(
        AlphaDividendsConsumer alphaDividendsConsumer,
        CalcYieldService calcYieldService,
        CalcGrowthService calcGrowth,
        ParseDataHelper parseDataHelper,
        ParseDecimalHelper parseDecimalHelper
        )
    {
        _alphaDividendsConsumer = alphaDividendsConsumer;
        _calcYieldService = calcYieldService;
        _calcGrowth = calcGrowth;
        _parseDataHelper = parseDataHelper;
        _parseDecimalHelper = parseDecimalHelper;
    }

    public async Task<DividendStatsResultDto> GenerateStats(string ticker, decimal precoAtual)
    {
        var dividendData = await _alphaDividendsConsumer.AlphaDividensConsumer();

        if (dividendData == null || dividendData.Data == null || !dividendData.Data.Any())
        {
            return new DividendStatsResultDto
            {
                Estatisticas = null,
                GrowthEntries = new List<DividendGrowthEntryDto>()
            };
        }

        var dividendGrowthEntries = new List<DividendGrowthEntryDto>();
        var dividendEntries = new List<DividendEntry>();

        for (int i = 0; i < dividendData.Data.Count; i++)
        {
            var currentEntry = dividendData.Data[i];

            // Base para estatísticas
            dividendEntries.Add(new DividendEntry
            {
                PaymentDate = currentEntry.PaymentDate,
                Amount = currentEntry.Amount
            });

            if (i > 0)
            {
                var previousEntry = dividendData.Data[i - 1];

                var growthVsPrevious = _parseDecimalHelper.ParseDecimal(previousEntry.Amount) != 0
                    ? ((_parseDecimalHelper.ParseDecimal(currentEntry.Amount) - _parseDecimalHelper.ParseDecimal(previousEntry.Amount)) / _parseDecimalHelper.ParseDecimal(previousEntry.Amount)) * 100
                    : 0;

                dividendGrowthEntries.Add(new DividendGrowthEntryDto
                {
                    PaymentDate = currentEntry.PaymentDate,
                    Amount = _parseDecimalHelper.ParseDecimal(currentEntry.Amount),
                    CrescimentoVsAnterior = growthVsPrevious
                });
            }
        }

        var estatisticas = new DividendStatisticsDto
        {
            Symbol = ticker,
            YieldAnualizado = _calcYieldService.CalcularYield(dividendEntries, precoAtual),
            CrescimentoDividendos = _calcGrowth.CalcularCrescimento(dividendEntries),
            Consistencia = CalcularConsistenciaDetalhada(dividendEntries),
            CalculadoEm = DateTime.UtcNow
        };

        return new DividendStatsResultDto
        {
            Estatisticas = estatisticas,
            GrowthEntries = dividendGrowthEntries
        };
    }

    public DividendConsistencyDto CalcularConsistenciaDetalhada(List<DividendEntry> dividendos)
    {
        if (dividendos == null || dividendos.Count < 2)
        {
            return new DividendConsistencyDto
            {
                TotalPagamentos = dividendos?.Count ?? 0,
                DiasNoPeriodo = 0,
                MediaDiasEntrePagamentos = 0,
                ConsistenciaPercentual = 0,
                Classificacao = "Irregular",
                AlertaGap = "Dados insuficientes"
            };
        }

        var ordenados = dividendos
            .OrderBy(d => DateTime.Parse(d.PaymentDate))
            .ToList();

        var datas = ordenados
            .Select(d => DateTime.Parse(d.PaymentDate))
            .ToList();

        int totalPagamentos = datas.Count;

        int diasNoPeriodo = (datas.Last() - datas.First()).Days;

        var intervalos = new List<int>();

        for (int i = 1; i < datas.Count; i++)
        {
            var diff = (datas[i] - datas[i - 1]).Days;
            intervalos.Add(diff);
        }

        double mediaDias = intervalos.Average();

        //  Consistência baseada na variação dos intervalos
        double desvio = intervalos.Select(i => Math.Abs(i - mediaDias)).Average();

        decimal consistenciaPercentual = mediaDias > 0
            ? (decimal)(100 - (desvio / mediaDias * 100))
            : 0;

        if (consistenciaPercentual < 0)
            consistenciaPercentual = 0;

        //  Classificação
        string classificacao = consistenciaPercentual switch
        {
            >= 85 => "Muito Regular",
            >= 60 => "Regular",
            _ => "Irregular"
        };

        //  Detectar GAP grande
        int maiorIntervalo = intervalos.Max();

        string? alertaGap = null;

        if (maiorIntervalo > mediaDias * 2)
        {
            alertaGap = $"Gap incomum detectado: {maiorIntervalo} dias entre pagamentos";
        }

        return new DividendConsistencyDto
        {
            TotalPagamentos = totalPagamentos,
            DiasNoPeriodo = diasNoPeriodo,
            MediaDiasEntrePagamentos = mediaDias,
            ConsistenciaPercentual = Math.Round(consistenciaPercentual, 2),
            Classificacao = classificacao,
            AlertaGap = alertaGap
        };
    }

    public class DividendStatsResultDto
    {
        public required DividendStatisticsDto Estatisticas { get; set; }
        public required List<DividendGrowthEntryDto> GrowthEntries { get; set; }
    }
}