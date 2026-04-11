namespace StatsEngineAPI.Domain.Dividends;

/// <summary>
/// Todas as métricas de yield de dividendos para dashboards e assessores.
/// </summary>
public class DividendYieldDto
{
    // ── Preço ────────────────────────────────────────────────────────────────
    public decimal PrecoAtual { get; set; }
    public decimal? PrecoMedioCompra { get; set; }

    // ── Yields principais ────────────────────────────────────────────────────

    /// <summary>Trailing Twelve Months — padrão do mercado financeiro</summary>
    public decimal YieldTtm { get; set; }

    /// <summary>Yield sobre todo o histórico disponível</summary>
    public decimal YieldHistorico { get; set; }

    /// <summary>Yield médio por mês nos últimos 12 meses</summary>
    public decimal YieldMensalMedio { get; set; }

    /// <summary>Projeção futura baseada nos últimos 3 pagamentos × frequência estimada</summary>
    public decimal YieldForward { get; set; }

    /// <summary>Yield sobre o preço médio de compra do cliente (Yield on Cost)</summary>
    public decimal? YieldOnCost { get; set; }

    // ── Totais em valor absoluto ─────────────────────────────────────────────
    public decimal TotalPagoTtm { get; set; }
    public decimal TotalPagoHistorico { get; set; }
    public decimal ProjecaoAnualValor { get; set; }

    // ── Frequência ───────────────────────────────────────────────────────────
    public int FrequenciaAnualEstimada { get; set; }
    public int MesesComPagamentoTtm { get; set; }

    // ── Último pagamento ─────────────────────────────────────────────────────
    public decimal UltimoPagamentoValor { get; set; }
    public DateTime UltimoPagamentoData { get; set; }

    // ── Extremos históricos ──────────────────────────────────────────────────
    public decimal MaiorPagamentoValor { get; set; }
    public DateTime? MaiorPagamentoData { get; set; }
    public decimal MenorPagamentoValor { get; set; }
    public DateTime? MenorPagamentoData { get; set; }

    // ── Sazonalidade ─────────────────────────────────────────────────────────
    public List<DividendSazonalidadeDto> Sazonalidade { get; set; } = new();

    // ── Classificações ───────────────────────────────────────────────────────
    public string ClassificacaoYieldTtm { get; set; } = string.Empty;
    public string ClassificacaoFrequencia { get; set; } = string.Empty;

    public static DividendYieldDto Vazio(decimal precoAtual) => new()
    {
        PrecoAtual = precoAtual,
        ClassificacaoYieldTtm = "Sem dados",
        ClassificacaoFrequencia = "Sem dados"
    };
}

public class DividendSazonalidadeDto
{
    public int Mes { get; set; }
    public string NomeMes { get; set; } = string.Empty;
    public decimal TotalPago { get; set; }
    public int QuantidadePagamentos { get; set; }
    public decimal MediaPorPagamento { get; set; }
}