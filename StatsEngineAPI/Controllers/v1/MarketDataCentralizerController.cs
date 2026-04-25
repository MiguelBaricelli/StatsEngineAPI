using Microsoft.AspNetCore.Mvc;
using StatsEngineAPI.Application.Services.Dividends.MarketDataCentralizer;

namespace StatsEngineAPI.Controllers.v1
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    public class MarketDataCentralizerController : ControllerBase
    {
        private readonly DividendsService _dividendsService;
        public MarketDataCentralizerController(DividendsService dividendsService)
        {
            _dividendsService = dividendsService;
        }

        [HttpGet("dividends/all/{symbol}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDividends(string symbol)
        {
            try
            {
                var result = await _dividendsService.GetDividendsData(symbol);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
