using StatsEngineAPI.Application.Helpers;
using StatsEngineAPI.Domain.Dividends;

namespace StatsEngineAPI.Application.Services.Dividends;

/// <summary>
/// Calcula consistência e regularidade de pagamentos de dividendos.
/// Remove a duplicação que existia entre DividendStatisticsService e GenerateStatsService.
/// </summary>
public class CalcConsistencyService
{
    private readonly ParseDecimalHelper _parseDecimalHelper;

    public CalcConsistencyService(ParseDecimalHelper parseDecimalHelper)
    {
        _parseDecimalHelper = parseDecimalHelper;
    }

    public DividendConsistencyDto CalcularConsistencia(
        List<DividendEntry> dividendos,
        int diasGapAlerta = 90)
    {
        if (dividendos == null || !dividendos.Any())
            return DividendConsistencyDto.SemDados();

        List<DividendEntry> paymentNoneList = dividendos
        .Where(r => r.PaymentDate != "None")
        .ToList();

        var ordenados = paymentNoneList
            .Where(d => !string.IsNullOrWhiteSpace(d.PaymentDate))
            .Select(d => new
            {
                Data = DateTime.Parse(d.PaymentDate),
                Valor = _parseDecimalHelper.ParseDecimal(d.Amount)
            })
            .OrderBy(d => d.Data)
            .ToList();

        if (ordenados.Count < 2)
            return DividendConsistencyDto.SemDados();

        var datas = ordenados.Select(d => d.Data).ToList();
        var diasNoPeriodo = (datas.Last() - datas.First()).Days;

        // ── Intervalos entre pagamentos ──────────────────────────────────────
        var intervalos = new List<int>();
        for (int i = 1; i < datas.Count; i++)
            intervalos.Add((datas[i] - datas[i - 1]).Days);

        var mediaDias = intervalos.Average();
        var maiorGap = intervalos.Max();
        var menorGap = intervalos.Min();

        // ── Consistência baseada no desvio dos intervalos ────────────────────
        // Quanto menor o desvio relativo, mais regular é o pagamento
        var desvioMedio = intervalos.Select(i => Math.Abs(i - mediaDias)).Average();
        var consistenciaPercentual = mediaDias > 0
            ? Math.Max(0, Math.Round((decimal)(100 - (desvioMedio / mediaDias * 100)), 2))
            : 0;

        // ── Streak de pagamentos ininterruptos (sem gap acima de 2× a média) ──
        var streakAtual = 0;
        var maiorStreak = 0;
        var correnteStreak = 1;

        for (int i = 0; i < intervalos.Count; i++)
        {
            if (intervalos[i] <= mediaDias * 2)
            {
                correnteStreak++;
                if (correnteStreak > maiorStreak)
                    maiorStreak = correnteStreak;
            }
            else
            {
                correnteStreak = 1;
            }
        }
        streakAtual = correnteStreak;

        // ── Gaps detectados acima do limite ──────────────────────────────────
        var gapsDetectados = new List<DividendGapDto>();
        for (int i = 1; i < datas.Count; i++)
        {
            var gap = (datas[i] - datas[i - 1]).Days;
            if (gap > diasGapAlerta)
            {
                gapsDetectados.Add(new DividendGapDto
                {
                    DataInicio = datas[i - 1],
                    DataFim = datas[i],
                    DiasDeGap = gap
                });
            }
        }

        // ── Pagamentos por ano ───────────────────────────────────────────────
        var pagamentosPorAno = ordenados
            .GroupBy(d => d.Data.Year)
            .ToDictionary(g => g.Key, g => g.Count());

        // ── Frequência detectada ─────────────────────────────────────────────
        var frequenciaDetectada = mediaDias switch
        {
            <= 35 => "Mensal",
            <= 50 => "Bimestral",
            <= 100 => "Trimestral",
            <= 200 => "Semestral",
            _ => "Anual"
        };

        return new DividendConsistencyDto
        {
            TotalPagamentos = ordenados.Count,
            DiasNoPeriodo = diasNoPeriodo,
            MediaDiasEntrePagamentos = Math.Round(mediaDias, 1),
            MaiorIntervaloDias = maiorGap,
            MenorIntervaloDias = menorGap,
            ConsistenciaPercentual = consistenciaPercentual,
            StreakAtualPagamentos = streakAtual,
            MaiorStreakHistorico = maiorStreak,
            QuantidadeGapsDetectados = gapsDetectados.Count,
            GapsDetectados = gapsDetectados,
            PagamentosPorAno = pagamentosPorAno,
            FrequenciaDetectada = frequenciaDetectada,
            Classificacao = ClassificarConsistencia(consistenciaPercentual),
            AlertaGap = gapsDetectados.Any()
                ? $"{gapsDetectados.Count} gap(s) detectado(s) — maior: {maiorGap} dias"
                : null
        };
    }

    private static string ClassificarConsistencia(decimal consistencia) => consistencia switch
    {
        >= 90 => "Muito Regular",
        >= 70 => "Regular",
        >= 50 => "Moderadamente Regular",
        _ => "Irregular"
    };
}