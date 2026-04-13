using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatsEngineAPI.Application.Services;
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
        try
        {
            if (precoAtual <= 0)
                precoAtual = 0;

            var dividendos = await _generateStats.GenerateStats(symbol, precoAtual);

            if (dividendos.Estatisticas == null || dividendos.GrowthEntries.Count == 0)
                return NotFound($"Nenhum dividendo encontrado para {symbol}.");

            return Ok(dividendos.Estatisticas);
        } catch (Exception e)
        {
            throw;
        }
        
    }

    /// <summary>
    /// Retorna as métricas de Dividend Yield (TTM, Forward, Mensal, Yield on Cost).
    /// </summary>
    [HttpGet("{symbol}/yield")]
    public async Task<IActionResult> GetYield(
        string symbol,
        [FromQuery] decimal precoAtual,
        [FromQuery] decimal? precoMedioCompra = null)
    {
        if (precoAtual <= 0)
            return BadRequest("Informe o preço atual do ativo (precoAtual > 0).");

        var result = await _generateStats.GenerateStats(symbol, precoAtual, precoMedioCompra);

        if (result.Estatisticas == null)
            return NotFound($"Nenhum dividendo encontrado para {symbol}.");

        return Ok(result.Estatisticas.Yield);
    }

    /// <summary>
    /// Retorna crescimento dos dividendos — CAGR, streak, variação YoY, volatilidade.
    /// </summary>
    [HttpGet("{symbol}/growth")]
    public async Task<IActionResult> GetGrowth(string symbol, [FromQuery] decimal precoAtual)
    {
        if (precoAtual <= 0)
            return BadRequest("Informe o preço atual do ativo (precoAtual > 0).");

        var result = await _generateStats.GenerateStats(symbol, precoAtual);

        if (result.Estatisticas == null)
            return NotFound($"Nenhum dividendo encontrado para {symbol}.");

        return Ok(result.Estatisticas.Crescimento);
    }

    /// <summary>
    /// Retorna consistência de pagamentos — regularidade, gaps, streak, frequência.
    /// </summary>
    [HttpGet("{symbol}/consistency")]
    public async Task<IActionResult> GetConsistency(string symbol, [FromQuery] decimal precoAtual)
    {
        if (precoAtual <= 0)
            return BadRequest("Informe o preço atual do ativo (precoAtual > 0).");

        var result = await _generateStats.GenerateStats(symbol, precoAtual);

        if (result.Estatisticas == null)
            return NotFound($"Nenhum dividendo encontrado para {symbol}.");

        return Ok(result.Estatisticas.Consistencia);
    }

    /// <summary>
    /// Retorna histórico de crescimento entrada a entrada — ideal para gráfico de linha.
    /// </summary>
    [HttpGet("{symbol}/growth/history")]
    public async Task<IActionResult> GetGrowthHistory(string symbol, [FromQuery] decimal precoAtual)
    {
        if (precoAtual <= 0)
            return BadRequest("Informe o preço atual do ativo (precoAtual > 0).");

        var result = await _generateStats.GenerateStats(symbol, precoAtual);

        if (result.GrowthEntries == null || !result.GrowthEntries.Any())
            return NotFound($"Nenhum histórico de crescimento encontrado para {symbol}.");

        return Ok(result.GrowthEntries);
    }

    /// <summary>
    /// Retorna sazonalidade — quais meses historicamente pagam mais dividendos.
    /// </summary>
    [HttpGet("{symbol}/seasonality")]
    public async Task<IActionResult> GetSeasonality(string symbol, [FromQuery] decimal precoAtual)
    {
        if (precoAtual <= 0)
            return BadRequest("Informe o preço atual do ativo (precoAtual > 0).");

        var result = await _generateStats.GenerateStats(symbol, precoAtual);

        if (result.Estatisticas == null)
            return NotFound($"Nenhum dividendo encontrado para {symbol}.");

        return Ok(result.Estatisticas.Yield.Sazonalidade);
    }

    /// <summary>
    /// Retorna crescimento anual agrupado por ano — ideal para gráfico de barras YoY.
    /// </summary>
    [HttpGet("{symbol}/growth/yearly")]
    public async Task<IActionResult> GetYearlyGrowth(string symbol, [FromQuery] decimal precoAtual)
    {
        if (precoAtual <= 0)
            return BadRequest("Informe o preço atual do ativo (precoAtual > 0).");

        var result = await _generateStats.GenerateStats(symbol, precoAtual);

        if (result.Estatisticas == null)
            return NotFound($"Nenhum dividendo encontrado para {symbol}.");

        return Ok(result.Estatisticas.Crescimento.PorAno);
    }

}
