using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StatsEngineAPI.Application.Services;
using StatsEngineAPI.Application.Services.MarketNews;

namespace StatsEngineAPI.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/market-intelligence")]
    public class MarketIntelligenceController : ControllerBase
    {

        private readonly NewsService _newsService;
        private readonly AssetAnalysisService _marketAnalysisService;

        public MarketIntelligenceController(NewsService newsService, AssetAnalysisService marketAnalysisService)
        {
            _newsService = newsService;
            _marketAnalysisService = marketAnalysisService;
        }

        // <summary>
        /// 1. Market Sentiment
        /// Retorna o sentimento geral do mercado (Bullish / Bearish / Neutral)
        /// calculado a partir da média dos scores das notícias recentes.
        /// </summary>
        /// <remarks>
        /// Ajuda o trader a entender a direção macro antes de abrir posições.
        /// 
        /// Regras de classificação:
        /// - Score médio > 0.2  → Bullish
        /// - Score médio &lt; -0.2 → Bearish
        /// - Demais              → Neutral
        /// </remarks>
        [HttpGet("market-sentiment")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> GetMarketSentiment(CancellationToken cancellationToken)
        {
            var result = await _newsService.GetMarketSentimentAsync(cancellationToken);

            return Ok(new
            {
                sentiment = result.Sentiment,
                score = result.Score,
                count = result.Count
            });
        }

        /// <summary>
        /// 2. Market Summary
        /// Retorna um resumo inteligente das notícias mais relevantes do mercado.
        /// Filtra por tickers importantes e ordena por score de relevância.
        /// </summary>
        /// <param name="top">Quantidade de notícias a incluir no resumo (padrão: 5)</param>
        /// <remarks>
        /// Fornece ao trader um panorama rápido do que realmente importa no mercado
        /// sem precisar ler todas as notícias disponíveis.
        /// </remarks>
        [HttpGet("market-summary")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> GetMarketSummary(
            CancellationToken cancellationToken,
            [FromQuery] int top = 5)
        {
            var result = await _newsService.GetMarketSummaryAsync(cancellationToken, top);

            return Ok(new
            {
                summary = result.Summary,
                total = result.Total,
                titles = result.Titles
            });
        }

        /// <summary>
        /// 3. Volatility Alert
        /// Detecta o nível de volatilidade do mercado com base na quantidade
        /// de notícias com alto impacto absoluto (|SentimentScore| > 0.5).
        /// </summary>
        /// <remarks>
        /// Níveis retornados:
        /// - NORMAL   → mercado estável, condições normais para operar
        /// - MODERATE → volatilidade moderada, monitore posições abertas
        /// - HIGH     → alta volatilidade, opere com cautela
        /// - EXTREME  → volatilidade extrema, evite operar ou use stops curtos
        /// </remarks>
        [HttpGet("volatility-alert")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> GetVolatilityAlert(CancellationToken cancellationToken)
        {
            var result = await _newsService.GetVolatilityAlertAsync(cancellationToken);

            return Ok(new
            {
                volatility = result.Volatility,
                highImpactNews = result.HighImpactNews,
                description = result.Description
            });
        }

        /// <summary>
        /// 4. Trade Signals
        /// Gera um sinal de operação (BUY / SELL / HOLD) baseado no sentimento
        /// ponderado pela relevância das notícias recentes e importantes.
        /// </summary>
        /// <remarks>
        /// Diferente de uma média simples, o sentimento ponderado dá mais peso
        /// às notícias mais relevantes para os tickers monitorados (SPX, USD, BRL…).
        /// 
        /// Regras de sinal:
        /// - Sentimento ponderado > 0.3  → BUY
        /// - Sentimento ponderado &lt; -0.3 → SELL
        /// - Demais                       → HOLD
        /// </remarks>
        [HttpGet("trade-signals")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> GetTradeSignals(CancellationToken cancellationToken)
        {
            var result = await _newsService.GetTradeSignalAsync(cancellationToken);

            return Ok(new
            {
                signal = result.Signal,
                sentiment = result.Sentiment,
                reasoning = result.Reasoning
            });
        }

        /// <summary>
        /// Análise completa do ativo.
        /// Retorna preço atual, notícias filtradas e ranqueadas, score de impacto,
        /// condição de mercado e sinal de operação.
        /// </summary>
        /// <param name="symbol">
        /// Ticker do ativo. Exemplos B3: PETR4, VALE3, ITUB4.
        /// Exemplos NYSE/NASDAQ: AAPL, MSFT, NVDA.
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <remarks>
        /// ### Campos retornados
        ///
        /// | Campo | Descrição |
        /// |---|---|
        /// | price | Preço atual, variação, máx/mín, volume |
        /// | assetScore.value | Score 0–1 de impacto das notícias no ativo |
        /// | assetScore.direction | BULLISH / BEARISH / NEUTRAL |
        /// | marketCondition.status | FAVORABLE / STABLE / VOLATILE / CRITICAL |
        /// | marketCondition.recommendation | BUY / SELL / HOLD / WAIT |
        /// | relevantNews | Top 5 notícias ranqueadas por relevância para o ativo |
        ///
        /// ### Lógica de detecção de mercado
        /// - Tickers com 5–6 chars (ex: PETR4, VALE3) → B3 via **brapi.dev**
        /// - Demais (ex: AAPL, MSFT) → NYSE/NASDAQ via **Alpha Vantage**
        /// </remarks>
        [HttpGet("analyze")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AnalyzeAsset(
            [FromQuery] string symbol,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                return BadRequest(new { error = "O parâmetro 'symbol' é obrigatório." });

            var result = await _marketAnalysisService.AnalyzeAssetAsync(
                symbol.Trim().ToUpper(),
                cancellationToken);

            return Ok(new
            {
                symbol = result.Symbol,
                assetName = result.AssetName,
                market = result.Market,
                analysisTimestamp = result.AnalysisTimestamp,

                price = new
                {
                    current = result.Price.Current,
                    change = result.Price.Change,
                    changePercent = result.Price.ChangePercent,
                    dayHigh = result.Price.DayHigh,
                    dayLow = result.Price.DayLow,
                    volume = result.Price.Volume,
                    currency = result.Price.Currency
                },

                marketCondition = new
                {
                    status = result.MarketCondition.Status,
                    description = result.MarketCondition.Description,
                    recommendation = result.MarketCondition.Recommendation
                },

                assetScore = new
                {
                    value = result.AssetScore.Value,
                    classification = result.AssetScore.Classification,
                    direction = result.AssetScore.Direction
                },

                relevantNews = result.RelevantNews.Select(n => new
                {
                    title = n.Title,
                    source = n.Source,
                    publishedAt = n.PublishedAt,
                    sentiment = n.Sentiment,
                    relevanceScore = n.RelevanceScore,
                    impactOnAsset = n.ImpactOnAsset,
                    url = n.Url
                })
            });
        }
    }
}

