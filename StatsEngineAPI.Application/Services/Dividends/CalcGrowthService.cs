using StatsEngineAPI.Application.Helpers;
using StatsEngineAPI.Domain.Dividends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Application.Services.Dividends
{
    public class CalcGrowthService
    {
        private readonly ParseDataHelper _parseDataHelper;
        private readonly ParseDecimalHelper _parseDecimalHelper;
        public CalcGrowthService(ParseDecimalHelper parseDecimalHelper,
            ParseDataHelper parseDataHelper
            )
        {
            _parseDecimalHelper = parseDecimalHelper;
            _parseDataHelper = parseDataHelper;

        }
        public DividendGrowthDto CalcularCrescimento(List<DividendEntry> dividendos)
        {
            var ordenados = dividendos.Where(d => !string.IsNullOrWhiteSpace(d.PaymentDate))
            .OrderBy(d => (d.PaymentDate))
            .ToList();

            if (ordenados.Count < 2)
            {
                var unico = _parseDecimalHelper.ParseDecimal(ordenados.FirstOrDefault()?.Amount ?? "0");
                return new DividendGrowthDto
                {
                    PrimeiroDividendo = unico,
                    UltimoDividendo = unico,
                    CrescimentoPercentual = 0,
                    Tendencia = "Insuficiente",
                    HistoricoCrescimento = new()
                };
            }

            var historico = new List<DividendGrowthEntryDto>();

            for (var i = 1; i < ordenados.Count; i++)
            {
                var anterior = _parseDecimalHelper.ParseDecimal(ordenados[i - 1].Amount);
                var atual = _parseDecimalHelper.ParseDecimal(ordenados[i].Amount);
                var crescimento = anterior == 0
                    ? 0
                    : Math.Round((atual - anterior) / anterior * 100, 2);

                historico.Add(new DividendGrowthEntryDto
                {
                    PaymentDate = ordenados[i].PaymentDate,
                    Amount = atual,
                    CrescimentoVsAnterior = crescimento
                });
            }

            var primeiro = _parseDecimalHelper.ParseDecimal(ordenados.First().Amount);
            var ultimo = _parseDecimalHelper.ParseDecimal(ordenados.Last().Amount);
            var crescimentoTotal = primeiro == 0
                ? 0
                : Math.Round((ultimo - primeiro) / primeiro * 100, 2);

            return new DividendGrowthDto
            {
                PrimeiroDividendo = primeiro,
                UltimoDividendo = ultimo,
                CrescimentoPercentual = crescimentoTotal,
                Tendencia = ClassificarTendencia(crescimentoTotal),
                HistoricoCrescimento = historico
            };
        }
        private static string ClassificarTendencia(decimal crescimento) => crescimento switch
        {
            > 5 => "Crescendo",
            < -5 => "Caindo",
            _ => "Estável"
        };

    }
}