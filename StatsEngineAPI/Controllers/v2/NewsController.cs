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

        [HttpGet("MostRecentAndRelevantNews")]
        public async Task<IActionResult> GetNewsMostRevelenceAsync(
            CancellationToken ct = default)
        {

            try
            {

                var bestNews = await _newsService.GetBestTradeNewsAsync(ct);

                if (bestNews == null || bestNews.Summary == null)
                    return NotFound($"Nenhuma notícia encontrada.");


                return Ok(bestNews);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpGet("LatestAndMostRelevantNews")]
        public async Task<IActionResult> GetNewsMostRevelenceListAsync(int take, CancellationToken ct = default)
        {
            try
            {

                if(take <= 0)
                {
                    return BadRequest("Precisa pegar mais que uma noticia");
                }

                var bestNews = await _newsService.GetRankedTradeNews(ct, take);

                if (bestNews == null || bestNews.Count == 0)
                    return NotFound($"Nenhuma notícia encontrada.");


                return Ok(bestNews);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}