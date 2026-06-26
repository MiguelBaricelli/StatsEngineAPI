using StatsEngineAPI.Application.Helpers;
using StatsEngineAPI.Domain.Dividends;

namespace StatsEngineAPI.Application.Services.Dividends;

/// <summary>
/// Calcula métricas de crescimento de dividendos — CAGR, streak, variações.
/// </summary>
public class CalcGrowthService
{
    private readonly ParseDecimalHelper _parseDecimalHelper;

    public CalcGrowthService(ParseDecimalHelper parseDecimalHelper)
    {
        _parseDecimalHelper = parseDecimalHelper;
    }

    public DividendGrowthDto CalcularCrescimento(List<DividendEntry> dividendos)
    {
        if (dividendos == null || dividendos.Count < 2)
            return DividendGrowthDto.Vazio();

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
            return DividendGrowthDto.Vazio();

        // ── Variação entre cada pagamento ────────────────────────────────────
        var entradas = new List<DividendGrowthEntryDto>();
        for (int i = 1; i < ordenados.Count; i++)
        {
            var anterior = ordenados[i - 1].Valor;
            var atual = ordenados[i].Valor;

            var crescimento = anterior != 0
                ? Math.Round((atual - anterior) / anterior * 100, 2)
                : 0;

            entradas.Add(new DividendGrowthEntryDto
            {
                PaymentDate = ordenados[i].Data.ToString("yyyy-MM-dd"),
                Amount = atual,
                CrescimentoVsAnterior = crescimento
            });
        }

        // ── CAGR — Compound Annual Growth Rate ───────────────────────────────
        var primeiro = ordenados.First();
        var ultimo = ordenados.Last();
        var anos = (ultimo.Data - primeiro.Data).TotalDays / 365.25;

        decimal cagr = 0;
        if (anos > 0 && primeiro.Valor > 0 && ultimo.Valor > 0)
        {
            cagr = Math.Round(
                ((decimal)Math.Pow((double)(ultimo.Valor / primeiro.Valor), 1.0 / anos) - 1) * 100,
                2
            );
        }

        // ── Crescimento acumulado total ──────────────────────────────────────
        var crescimentoTotal = primeiro.Valor > 0
            ? Math.Round((ultimo.Valor - primeiro.Valor) / primeiro.Valor * 100, 2)
            : 0;

        // ── Crescimento médio entre pagamentos ───────────────────────────────
        var mediaCrescimento = entradas.Any()
            ? Math.Round((decimal)entradas.Average(e => e.CrescimentoVsAnterior), 2)
            : 0;

        // ── Streak — quantos pagamentos consecutivos com crescimento ─────────
        var streak = CalcularStreak(entradas);

        // ── Agrupamento anual ────────────────────────────────────────────────
        var porAno = ordenados
            .GroupBy(d => d.Data.Year)
            .Select(g => new DividendAnualDto
            {
                Ano = g.Key,
                TotalPago = Math.Round(g.Sum(d => d.Valor), 4),
                QuantidadePagamentos = g.Count(),
                MediaPorPagamento = Math.Round((decimal)g.Average(d => d.Valor), 4)
            })
            .OrderBy(a => a.Ano)
            .ToList();

        // Crescimento anual YoY
        for (int i = 1; i < porAno.Count; i++)
        {
            var anoAnterior = porAno[i - 1].TotalPago;
            porAno[i].CrescimentoVsAnoAnterior = anoAnterior > 0
                ? Math.Round((porAno[i].TotalPago - anoAnterior) / anoAnterior * 100, 2)
                : 0;
        }

        // ── Volatilidade dos pagamentos ──────────────────────────────────────
        var valores = ordenados.Select(d => d.Valor).ToList();
        var mediaValores = valores.Average();
        var desvioPadrao = Math.Sqrt(
            (double)valores.Select(v => (v - (decimal)mediaValores) * (v - (decimal)mediaValores)).Average()
        );
        var coeficienteVariacao = mediaValores > 0
            ? Math.Round((decimal)(desvioPadrao / (double)mediaValores) * 100, 2)
            : 0;

        return new DividendGrowthDto
        {
            // CAGR e crescimentos
            Cagr = cagr,
            CrescimentoTotalPercentual = crescimentoTotal,
            MediaCrescimentoEntrePagamentos = mediaCrescimento,

            // Streak
            StreakAtualCrescimento = streak.StreakAtual,
            MaiorStreakHistorico = streak.MaiorStreak,

            // Extremos
            MaiorCrescimento = entradas.Any() ? entradas.Max(e => e.CrescimentoVsAnterior) : 0,
            MenorCrescimento = entradas.Any() ? entradas.Min(e => e.CrescimentoVsAnterior) : 0,

            // Volatilidade
            CoeficienteVariacaoPagamentos = coeficienteVariacao,
            ClassificacaoVolatilidade = ClassificarVolatilidade(coeficienteVariacao),

            // Período
            PrimeiroPagamentoData = primeiro.Data,
            UltimoPagamentoData = ultimo.Data,
            PrimeiroPagamentoValor = primeiro.Valor,
            UltimoPagamentoValor = ultimo.Valor,
            AnosDeHistorico = Math.Round((decimal)anos, 1),

            // Detalhes
            Entradas = entradas,
            PorAno = porAno,

            // Classificação
            ClassificacaoCrescimento = ClassificarCrescimento(cagr),
        };
    }

    private static (int StreakAtual, int MaiorStreak) CalcularStreak(List<DividendGrowthEntryDto> entradas)
    {
        int streakAtual = 0, maiorStreak = 0, corrente = 0;

        foreach (var entrada in entradas)
        {
            if (entrada.CrescimentoVsAnterior > 0)
            {
                corrente++;
                if (corrente > maiorStreak)
                    maiorStreak = corrente;
            }
            else
            {
                corrente = 0;
            }
        }

        streakAtual = corrente;
        return (streakAtual, maiorStreak);
    }

    private static string ClassificarCrescimento(decimal cagr) => cagr switch
    {
        <= 0 => "Declinante",
        < 3 => "Fraco",
        < 7 => "Moderado",
        < 12 => "Forte",
        _ => "Excepcional"
    };

    private static string ClassificarVolatilidade(decimal cv) => cv switch
    {
        < 10 => "Muito Estável",
        < 25 => "Estável",
        < 50 => "Moderada",
        _ => "Alta Volatilidade"
    };
}
