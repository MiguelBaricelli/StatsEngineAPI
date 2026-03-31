using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatsEngineAPI.Application.Services.Dividends;
using StatsEngineAPI.Domain.Interfaces.Services.Dividends;

namespace MarketDataCentralizer.Controllers.v1.Dividends;


[ApiController]
[Route("api/[controller]")]
public class DividendStatisticsController : ControllerBase
{
    private readonly GenerateStatsService _generateStats;

    public DividendStatisticsController(
        GenerateStatsService generateStatsService)
    {
        _generateStats = generateStatsService;
    }

    /// <summary>
    /// Retorna todas as estatísticas de dividendos de um ativo.
    /// </summary>
    [HttpGet("{symbol}/statistics")]
    public async Task<IActionResult> GetStatistics(string symbol, [FromQuery] decimal precoAtual)
    {
        if (precoAtual <= 0)
            return BadRequest("Informe o preço atual do ativo (precoAtual > 0).");

        var dividendos = await _generateStats.GenerateStats(symbol, precoAtual);

        if (dividendos.Estatisticas == null || dividendos.GrowthEntries.Count == 0)
            return NotFound($"Nenhum dividendo encontrado para {symbol}.");

        return Ok(dividendos.Estatisticas);
    }

    /// <summary>
    /// Retorna apenas o Dividend Yield anualizado.
    /// </summary>
    [HttpGet("{symbol}/yield")]
    public async Task<IActionResult> GetYield(string symbol, [FromQuery] decimal precoAtual)
    {
        if (precoAtual <= 0)
            return BadRequest("Informe o preço atual do ativo (precoAtual > 0).");

        var dividendos = await _generateStats.GenerateStats(symbol, precoAtual);


        if (dividendos.Estatisticas == null || dividendos.GrowthEntries.Count == 0)
            return NotFound($"Nenhum dividendo encontrado para {symbol}.");

        return Ok(dividendos.GrowthEntries);
    }

}
