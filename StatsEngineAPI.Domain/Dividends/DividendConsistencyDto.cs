namespace StatsEngineAPI.Domain.Dividends;

/// <summary>
/// Métricas de consistência e regularidade de pagamentos.
/// </summary>
public class DividendConsistencyDto
{
    public int TotalPagamentos { get; set; }
    public int DiasNoPeriodo { get; set; }

    // ── Intervalos ───────────────────────────────────────────────────────────
    public double MediaDiasEntrePagamentos { get; set; }
    public int MaiorIntervaloDias { get; set; }
    public int MenorIntervaloDias { get; set; }

    // ── Consistência ─────────────────────────────────────────────────────────
    public decimal ConsistenciaPercentual { get; set; }
    public string Classificacao { get; set; } = string.Empty;
    public string FrequenciaDetectada { get; set; } = string.Empty;

    // ── Streak ───────────────────────────────────────────────────────────────
    public int StreakAtualPagamentos { get; set; }
    public int MaiorStreakHistorico { get; set; }

    // ── Gaps ─────────────────────────────────────────────────────────────────
    public int QuantidadeGapsDetectados { get; set; }
    public string? AlertaGap { get; set; }
    public List<DividendGapDto> GapsDetectados { get; set; } = new();

    // ── Histórico anual ──────────────────────────────────────────────────────
    public Dictionary<int, int> PagamentosPorAno { get; set; } = new();

    public static DividendConsistencyDto SemDados() => new()
    {
        Classificacao = "Sem dados",
        FrequenciaDetectada = "Sem dados",
        AlertaGap = "Dados insuficientes para análise"
    };
}

public class DividendGapDto
{
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public int DiasDeGap { get; set; }
}

