namespace StatsEngineAPI.Domain.Dividends;

/// <summary>
/// Métricas de crescimento de dividendos para dashboards e assessores.
/// </summary>
public class DividendGrowthDto
{
    // ── CAGR e crescimentos ──────────────────────────────────────────────────

    /// <summary>Compound Annual Growth Rate — crescimento anual composto</summary>
    public decimal Cagr { get; set; }

    /// <summary>Crescimento percentual do primeiro ao último pagamento</summary>
    public decimal CrescimentoTotalPercentual { get; set; }

    /// <summary>Média de crescimento entre cada pagamento consecutivo</summary>
    public decimal MediaCrescimentoEntrePagamentos { get; set; }

    // ── Streak de crescimento ────────────────────────────────────────────────

    /// <summary>Quantos pagamentos consecutivos crescendo atualmente</summary>
    public int StreakAtualCrescimento { get; set; }

    /// <summary>Maior sequência de crescimentos consecutivos já registrada</summary>
    public int MaiorStreakHistorico { get; set; }

    // ── Extremos ─────────────────────────────────────────────────────────────
    public decimal MaiorCrescimento { get; set; }
    public decimal MenorCrescimento { get; set; }

    // ── Volatilidade ─────────────────────────────────────────────────────────

    /// <summary>Coeficiente de variação dos pagamentos em % — quanto oscilam</summary>
    public decimal CoeficienteVariacaoPagamentos { get; set; }
    public string ClassificacaoVolatilidade { get; set; } = string.Empty;

    // ── Período ──────────────────────────────────────────────────────────────
    public DateTime PrimeiroPagamentoData { get; set; }
    public DateTime UltimoPagamentoData { get; set; }
    public decimal PrimeiroPagamentoValor { get; set; }
    public decimal UltimoPagamentoValor { get; set; }
    public decimal AnosDeHistorico { get; set; }

    // ── Classificações ───────────────────────────────────────────────────────
    public string ClassificacaoCrescimento { get; set; } = string.Empty;

    // ── Detalhes ─────────────────────────────────────────────────────────────
    public List<DividendGrowthEntryDto> Entradas { get; set; } = new();
    public List<DividendAnualDto> PorAno { get; set; } = new();

    public static DividendGrowthDto Vazio() => new()
    {
        ClassificacaoCrescimento = "Sem dados",
        ClassificacaoVolatilidade = "Sem dados"
    };
}

public class DividendAnualDto
{
    public int Ano { get; set; }
    public decimal TotalPago { get; set; }
    public int QuantidadePagamentos { get; set; }
    public decimal MediaPorPagamento { get; set; }
    public decimal CrescimentoVsAnoAnterior { get; set; }
}