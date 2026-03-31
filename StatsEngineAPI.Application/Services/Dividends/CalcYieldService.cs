using StatsEngineAPI.Application.Helpers;
using StatsEngineAPI.Domain.Dividends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Application.Services.Dividends
{
    public class CalcYieldService 
    {
        private readonly ParseDataHelper _parseDataHelper;
        private readonly ParseDecimalHelper _parseDecimalHelper;
        public CalcYieldService(ParseDecimalHelper parseDecimalHelper,
            ParseDataHelper parseDataHelper
            )
        {
            _parseDecimalHelper = parseDecimalHelper;
            _parseDataHelper = parseDataHelper;
           
        }
        public DividendYieldDto CalcularYield(List<DividendEntry> dividendos, decimal precoAtual)
        {
            if (precoAtual <= 0)
                throw new ArgumentException("Preço atual deve ser maior que zero.", nameof(precoAtual));

            var ordenados = dividendos.Where(d => !string.IsNullOrWhiteSpace(d.PaymentDate))
           .OrderBy(d => (d.PaymentDate))
           .ToList();
            var total = ordenados.Sum(d => _parseDecimalHelper.ParseDecimal(d.Amount));
            var yield = Math.Round(total / precoAtual * 100, 2);

            return new DividendYieldDto
            {
                TotalPagoNoPeriodo = total,
                PrecoAtual = precoAtual,
                YieldPercentual = yield,
                Classificacao = ClassificarYield(yield)
            };
        }

        private static string ClassificarYield(decimal yield) => yield switch
        {
            < 2 => "Baixo",
            < 6 => "Moderado",
            _ => "Alto"
        };
    }

}
