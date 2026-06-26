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
        private readonly IbovespaService _ibovespaService;
        public MarketDataCentralizerController(DividendsService dividendsService, IbovespaService ibovespaService)
        {
            _dividendsService = dividendsService;
            _ibovespaService = ibovespaService;
        }

        [HttpGet("dividends/all/{symbol}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDividends(string symbol, CancellationToken cancellationToken)
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

        [HttpGet("ibovespa/{symbol}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAssetIbov(string symbol, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _ibovespaService.GetIbovespaData(symbol, cancellationToken);

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
