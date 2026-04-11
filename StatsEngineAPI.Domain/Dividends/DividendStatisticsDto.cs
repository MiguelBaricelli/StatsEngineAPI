namespace StatsEngineAPI.Domain.Dividends;

public class DividendStatisticsDto
{
    public string Symbol { get; set; } = string.Empty;
    public DividendYieldDto Yield { get; set; } = new();
    public DividendGrowthDto Crescimento { get; set; } = new();
    public DividendConsistencyDto Consistencia { get; set; } = new();
    public DateTime CalculadoEm { get; set; }
}
