using StatsEngineAPI.Application.Helpers;
using StatsEngineAPI.Domain.Dividends;

namespace StatsEngineAPI.Application.Services.Dividends;

/// <summary>
/// Calcula todas as métricas de Dividend Yield relevantes para assessores e dashboards.
/// </summary>
public class CalcYieldService
{
    private readonly ParseDecimalHelper _parseDecimalHelper;

    public CalcYieldService(ParseDecimalHelper parseDecimalHelper)
    {
        _parseDecimalHelper = parseDecimalHelper;
    }

    public DividendYieldDto CalcularYield(List<DividendEntry> dividendos, decimal precoAtual, decimal? precoMedioCompra = null)
    {
        if (precoAtual <= 0)
            throw new ArgumentException("Preço atual deve ser maior que zero.", nameof(precoAtual));

        if (dividendos == null || !dividendos.Any())
            return DividendYieldDto.Vazio(precoAtual);

        var ordenados = dividendos
            .Where(d => !string.IsNullOrWhiteSpace(d.PaymentDate))
            .Select(d => new
            {
                Entry = d,
                Data = DateTime.Parse(d.PaymentDate),
                Valor = _parseDecimalHelper.ParseDecimal(d.Amount)
            })
            .OrderBy(d => d.Data)
            .ToList();

        if (!ordenados.Any())
            return DividendYieldDto.Vazio(precoAtual);

        var agora = DateTime.UtcNow;

        // ── Trailing Twelve Months (TTM) ─────────────────────────────────────
        // Padrão do mercado: soma dos últimos 12 meses dividida pelo preço atual
        var ttmInicio = agora.AddMonths(-12);
        var totalTtm = ordenados
            .Where(d => d.Data >= ttmInicio)
            .Sum(d => d.Valor);

        var yieldTtm = precoAtual > 0
            ? Math.Round(totalTtm / precoAtual * 100, 2)
            : 0;

        // ── Total histórico ──────────────────────────────────────────────────
        var totalHistorico = ordenados.Sum(d => d.Valor);
        var yieldHistorico = Math.Round(totalHistorico / precoAtual * 100, 2);

        // ── Yield mensal médio (TTM) ─────────────────────────────────────────
        var mesesComPagamento = ordenados
            .Where(d => d.Data >= ttmInicio)
            .Select(d => new { d.Data.Year, d.Data.Month })
            .Distinct()
            .Count();

        var yieldMensalMedio = mesesComPagamento > 0
            ? Math.Round(yieldTtm / mesesComPagamento, 4)
            : 0;

        // ── Projeção anual (forward yield) ───────────────────────────────────
        // Usa a média dos últimos 3 pagamentos como estimativa do próximo
        var ultimos3 = ordenados.TakeLast(3).ToList();
        var mediaPagamento = ultimos3.Any()
            ? ultimos3.Average(d => d.Valor)
            : 0;

        // Frequência estimada de pagamentos por ano
        var frequenciaAnual = EstimarFrequenciaAnual(ordenados.Select(d => d.Data).ToList());
        var projecaoAnualValor = (decimal)mediaPagamento * frequenciaAnual;
        var yieldForward = precoAtual > 0
            ? Math.Round(projecaoAnualValor / precoAtual * 100, 2)
            : 0;

        // ── Yield on Cost ────────────────────────────────────────────────────
        // Retorno sobre o preço médio de compra (relevante para carteira do cliente)
        decimal? yieldOnCost = null;
        if (precoMedioCompra.HasValue && precoMedioCompra > 0)
            yieldOnCost = Math.Round(totalTtm / precoMedioCompra.Value * 100, 2);

        // ── Maior e menor pagamento ──────────────────────────────────────────
        var maiorPagamento = ordenados.MaxBy(d => d.Valor);
        var menorPagamento = ordenados.MinBy(d => d.Valor);

        // ── Pagamento mais recente ───────────────────────────────────────────
        var ultimoPagamento = ordenados.Last();

        // ── Sazonalidade — meses que historicamente pagam mais ───────────────
        var sazonalidade = ordenados
            .GroupBy(d => d.Data.Month)
            .Select(g => new DividendSazonalidadeDto
            {
                Mes = g.Key,
                NomeMes = new DateTime(2000, g.Key, 1).ToString("MMM"),
                TotalPago = Math.Round(g.Sum(d => d.Valor), 4),
                QuantidadePagamentos = g.Count(),
                MediaPorPagamento = Math.Round((decimal)g.Average(d => d.Valor), 4)
            })
            .OrderBy(s => s.Mes)
            .ToList();

        return new DividendYieldDto
        {
            // Preço
            PrecoAtual = precoAtual,
            PrecoMedioCompra = precoMedioCompra,

            // Yields principais
            YieldTtm = yieldTtm,
            YieldHistorico = yieldHistorico,
            YieldMensalMedio = yieldMensalMedio,
            YieldForward = yieldForward,
            YieldOnCost = yieldOnCost,

            // Totais
            TotalPagoTtm = totalTtm,
            TotalPagoHistorico = totalHistorico,
            ProjecaoAnualValor = Math.Round(projecaoAnualValor, 4),

            // Frequência
            FrequenciaAnualEstimada = frequenciaAnual,
            MesesComPagamentoTtm = mesesComPagamento,

            // Último pagamento
            UltimoPagamentoValor = ultimoPagamento.Valor,
            UltimoPagamentoData = ultimoPagamento.Data,

            // Extremos históricos
            MaiorPagamentoValor = maiorPagamento?.Valor ?? 0,
            MaiorPagamentoData = maiorPagamento?.Data,
            MenorPagamentoValor = menorPagamento?.Valor ?? 0,
            MenorPagamentoData = menorPagamento?.Data,

            // Sazonalidade
            Sazonalidade = sazonalidade,

            // Classificação
            ClassificacaoYieldTtm = ClassificarYield(yieldTtm),
            ClassificacaoFrequencia = ClassificarFrequencia(frequenciaAnual),
        };
    }

    // ── Estima quantas vezes por ano a empresa paga dividendos ───────────────
    private static int EstimarFrequenciaAnual(List<DateTime> datas)
    {
        if (datas.Count < 2) return 1;

        var intervalos = new List<double>();
        for (int i = 1; i < datas.Count; i++)
            intervalos.Add((datas[i] - datas[i - 1]).TotalDays);

        var mediaIntervalo = intervalos.Average();

        return mediaIntervalo switch
        {
            <= 35 => 12,   // mensal
            <= 50 => 6,    // bimestral
            <= 100 => 4,   // trimestral
            <= 200 => 2,   // semestral
            _ => 1          // anual
        };
    }

    private static string ClassificarYield(decimal yield) => yield switch
    {
        <= 0 => "Sem rendimento",
        < 2 => "Baixo",
        < 4 => "Moderado",
        < 8 => "Bom",
        < 12 => "Alto",
        _ => "Muito Alto — verificar sustentabilidade"
    };

    private static string ClassificarFrequencia(int frequencia) => frequencia switch
    {
        12 => "Mensal",
        6 => "Bimestral",
        4 => "Trimestral",
        2 => "Semestral",
        _ => "Anual"
    };
}
