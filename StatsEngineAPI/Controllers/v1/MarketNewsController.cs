using Microsoft.AspNetCore.Mvc;
using StatsEngineAPI.Application.Services.MarketNews;
using StatsEngineAPI.Domain.MarketNews;

namespace StatsEngineAPI.Controllers.V1
{
    [ApiController]
    [Route("api/market-news")]
    public class MarketNewsController : ControllerBase
    {
        private readonly MarketNewsService _marketNewsService;

        public MarketNewsController(MarketNewsService marketNewsService)
        {
            _marketNewsService = marketNewsService;
        }

        [HttpGet("sentiment")]
        public async Task<IActionResult> GetSentiment(
       [FromQuery] MarketNewsQueryParams query,
       CancellationToken ct)
        {
            var result = await _marketNewsService.GetSentimentAsync(query, ct);
            return Ok(result);
        }

        /// <summary>
        /// Retorna as notícias com maior sentimento positivo ou negativo.
        /// </summary>
        /// <param name="limit">Quantidade de resultados</param>
        /// <param name="type">positive | negative</param>
        [HttpGet("sentiment/filtred")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBySentiment(
        [FromQuery] int limit = 5,
        [FromQuery] string type = "positive",
        CancellationToken ct = default)
        {
            var result = await _marketNewsService.GetTopSentimentAsync(limit, type, ct);

            return Ok(result);
        }

        /// <summary>
        /// Retorna notícias com score baseado em sentimento + volume
        /// </summary>
        [HttpGet("score")]
        public async Task<IActionResult> GetScore(
            [FromQuery] int limit = 5,
            CancellationToken ct = default)
        {
            var result = await _marketNewsService.GetNewsScoreAsync(limit, ct);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

    [HttpGet("ticker/{symbol}")]
        public async Task<IActionResult> GetByTicker(
            string symbol,
            [FromQuery] string? publishedOn,
            [FromQuery] double? sentimentGte,
            [FromQuery] double? sentimentLte,
            [FromQuery] string? sort,
            [FromQuery] string? sortOrder,
            [FromQuery] int? limit,
            CancellationToken ct)
        {
            var result = await _marketNewsService.GetByTickerAsync(
                symbol, publishedOn, sentimentGte, sentimentLte, sort, sortOrder, limit, ct);

            return Ok(result);
        }

        [HttpGet("article/{uuid}")]
        public async Task<IActionResult> GetArticle(string uuid, CancellationToken ct)
        {
            var result = await _marketNewsService.GetArticleAsync(uuid, ct);
            return Ok(result);
        }

        [HttpGet("similar/{uuid}")]
        public async Task<IActionResult> GetSimilar(
            string uuid,
            [FromQuery] string? language,
            [FromQuery] string? publishedOn,
            CancellationToken ct)
        {
            var result = await _marketNewsService.GetSimilarAsync(uuid, language, publishedOn, ct);
            return Ok(result);
        }
    }
}
