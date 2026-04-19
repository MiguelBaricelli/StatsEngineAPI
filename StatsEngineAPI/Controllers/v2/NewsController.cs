using Microsoft.AspNetCore.Mvc;
using StatsEngineAPI.Application.Services.MarketNews;

namespace StatsEngineAPI.Controllers.v2
{
    [ApiController]
    [Route("api/v2/market-news")]
    public class NewsController : ControllerBase
    {
        private readonly ILogger<NewsController> _logger;
        private readonly NewsService _newsService;
        public NewsController(NewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            string symbol = null,
            CancellationToken ct = default)
        {

            try
            {
                var result = await _newsService.GetLatestNewsAsync(symbol, ct);

                if (result == null || result.Feed == null || result.Feed.Count == 0)
                    return NotFound($"Nenhuma notícia encontrada para {symbol}.");

                return Ok(result);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }
    }
}