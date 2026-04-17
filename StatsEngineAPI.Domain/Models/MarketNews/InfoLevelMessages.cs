namespace StatsEngineAPI.Domain.Models.MarketNews
{
    public class InfoLevelMessages
    {

        public string GetLevels(double score)
        {
            return score switch
            {
                >= 2.0 => "MUITO ALTA: Forte pressão positiva no mercado. Possível oportunidade de COMPRA. Fique atento a rompimentos e aumento de volume.",

                >= 1.5 => "ALTA FORTE: Sentimento bem positivo. Mercado pode estar entrando em tendência de alta. Avalie entradas, mas evite entrar atrasado (FOMO).",

                >= 1.2 => "ALTA: Viés positivo identificado. Boa oportunidade, mas confirme com análise técnica antes de entrar.",

                >= 0.8 => "MÉDIA-ALTA: Movimento começando a ganhar força. Fique atento — pode evoluir para uma tendência.",

                >= 0.5 => "MÉDIA: Mercado neutro com leve viés. Melhor aguardar confirmação antes de operar.",

                >= 0.2 => "BAIXA: Pouco impacto de notícias. Cenário fraco para tomada de decisão baseada em sentimento.",

                >= 0.0 => "MUITO BAIXA: Quase sem relevância. Evite operar baseado em notícias — risco de ruído alto.",

                >= -0.5 => "MÉDIA NEGATIVA: Viés de queda leve. Cuidado com compras. Possível pressão vendedora.",

                >= -1.0 => "NEGATIVA: Sentimento negativo relevante. Avalie oportunidades de venda ou evite posições compradas.",

                >= -1.5 => " NEGATIVA FORTE: Forte pressão vendedora. Alto risco para compras. Mercado pode continuar caindo.",

                < -1.5 => "MUITO NEGATIVA: Impacto extremo negativo. Possível pânico ou notícias críticas. Evite operar contra a tendência.",

                _ => "Sem relevância"
            };
        }
    }
}
