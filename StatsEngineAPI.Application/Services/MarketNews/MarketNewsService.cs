using StatsEngineAPI.Domain.MarketNews;
using StatsEngineAPI.Domain.Models.MarketNews;
using StatsEngineAPI.Infrastructure.Repository.MarketNews;

namespace StatsEngineAPI.Application.Services.MarketNews
{
    public class MarketNewsService
    {
        private readonly MarketNewsIntegration _marketNewsIntegration;
        private readonly InfoLevelMessages _infoLevelMessages;

        public MarketNewsService(MarketNewsIntegration marketNewsIntegration, InfoLevelMessages infoLevelMessages)
        {
            _marketNewsIntegration = marketNewsIntegration;
            _infoLevelMessages = infoLevelMessages;
        }

        /// <summary>
        /// Busca notícias de mercado com base em parâmetros genéricos de consulta.
        /// 
        /// Esse método aplica configurações padrão para garantir consistência:
        /// - Language: inglês ("en")
        /// - Ordenação por sentimento da entidade (entity_sentiment_score)
        /// - Ordem decrescente (notícias mais relevantes primeiro)
        /// - Filtra apenas notícias que possuem entidades relevantes
        /// 
        /// Ideal para obter uma visão geral do sentimento do mercado.
        /// </summary>
        /// <param name="query">Parâmetros de busca das notícias</param>
        /// <param name="ct">Token de cancelamento da requisição</param>
        /// <returns>Objeto contendo dados das notícias retornadas pela API</returns>
        public async Task<object> GetSentimentAsync(
            MarketNewsQueryParams query,
            CancellationToken ct)
        {
            query.Language ??= "en";
            query.Sort ??= "entity_sentiment_score";
            query.SortOrder ??= "desc";
            query.FilterEntities ??= true;
            query.MustHaveEntities ??= true;

            return await _marketNewsIntegration.GetNewsAsync(query, ct);
        }

        /// <summary>
        /// Retorna notícias com maior sentimento positivo ou negativo,
        /// incluindo dados completos da notícia e da entidade.
        /// </summary>
        public async Task<IEnumerable<object>> GetTopSentimentAsync(
            int limit,
            string type,
            CancellationToken ct)
        {
            var query = new MarketNewsQueryParams
            {
                Language = "en",
                MustHaveEntities = true,
                FilterEntities = true,
                Limit = 50
            };

            var news = await _marketNewsIntegration.GetNewsAsync(query, ct);

            var result = news.Data
                .SelectMany(n => n.Entities.Select(e => new
                {
                    // Dados da notícia
                    n.Uuid,
                    n.Title,
                    n.Description,
                    n.PublishedAt,
                    e.Industry,


                    // Dados da entidade
                    e.Symbol,
                    e.Name,
                    e.SentimentScore,
                    e.Highlights,
                }))
                .Where(x => x.SentimentScore != null);

            var filtered = type.ToLower() switch
            {
                "positive" => result
                    .OrderByDescending(x => x.SentimentScore),

                "negative" => result
                    .OrderBy(x => x.SentimentScore),

                _ => result
                    .OrderByDescending(x => Math.Abs(x.SentimentScore))
            };

            return filtered.Take(limit);
        }

        /// <summary>
        /// Retorna notícias com score baseado em sentimento + volume
        /// </summary>
        public async Task<IEnumerable<object>> GetNewsScoreAsync(
            int limit,
            CancellationToken ct)
        {
            var query = new MarketNewsQueryParams
            {
                Language = "en",
                MustHaveEntities = true,
                FilterEntities = true,
                Limit = 100
            };

            var news = await _marketNewsIntegration.GetNewsAsync(query, ct);

            // Flatten (notícia + entidade)
            var flat = news.Data
                .SelectMany(n => n.Entities.Select(e => new
                {
                    n.Title,
                    n.Description,
                    n.PublishedAt,
                    n.Uuid,
                    e.Symbol,
                    Sentiment = e.SentimentScore
                }));

            //REMOVE DUPLICIDADE POR UUID(mantém maior sentimento da notícia)
            var uniqueNews = flat
               .GroupBy(x => x.Uuid)
               .Select(g => g.OrderByDescending(x => x.Sentiment).First());

            // Agrupar por ativo
            var grouped = uniqueNews
                .GroupBy(x => x.Symbol)
                .Select(g =>
               {
                   var avgSentiment = g.Average(x => x.Sentiment);
                   var volume = g.Count();

                   var volumeScore = Math.Log(volume + 1);
                   var finalScore = avgSentiment + volumeScore;

                   var topNews = g
                       .OrderByDescending(x => x.Sentiment)
                       .First();



                   return new
                   {
                       topNews.Uuid,
                       topNews.Title,
                       topNews.Description,
                       topNews.PublishedAt,
                       Score = Math.Round(finalScore, 3),
                       Level = _infoLevelMessages.GetLevels(finalScore)
                   };
               })
                .OrderByDescending(x => x.Score)
                .Take(limit);

            return grouped;
        }



        /// <summary>
        /// Busca notícias filtradas por um ticker específico (ex: AAPL, TSLA).
        /// 
        /// Permite aplicar filtros adicionais como:
        /// - Data de publicação
        /// - Faixa de sentimento (mínimo e máximo)
        /// - Ordenação personalizada
        /// - Limite de resultados
        /// 
        /// Muito útil para estratégias de trade baseadas em sentimento de ativos específicos.
        /// </summary>
        /// <param name="symbol">Ticker do ativo (ex: "AAPL")</param>
        /// <param name="publishedOn">Data da notícia (formato yyyy-MM-dd)</param>
        /// <param name="sentimentGte">Sentimento mínimo (greater than or equal)</param>
        /// <param name="sentimentLte">Sentimento máximo (less than or equal)</param>
        /// <param name="sort">Campo para ordenação</param>
        /// <param name="sortOrder">Ordem (asc ou desc)</param>
        /// <param name="limit">Quantidade máxima de resultados</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>Lista de notícias filtradas pelo ticker</returns>
        public async Task<object> GetByTickerAsync(
                string symbol,
                string? publishedOn,
                double? sentimentGte,
                double? sentimentLte,
                string? sort,
                string? sortOrder,
                int? limit,
                CancellationToken ct)
        {
            var query = new MarketNewsQueryParams
            {
                Symbols = symbol,
                Language = "en",
                PublishedOn = publishedOn,
                SentimentGte = sentimentGte,
                SentimentLte = sentimentLte,
                Sort = sort ?? "entity_sentiment_score",
                SortOrder = sortOrder ?? "desc",
                Limit = limit,
                FilterEntities = true,
                MustHaveEntities = true
            };

            return await _marketNewsIntegration.GetNewsAsync(query, ct);
        }

        /// <summary>
        /// Busca uma notícia específica através do seu UUID.
        /// 
        /// Cada notícia na API possui um identificador único (UUID),
        /// permitindo recuperar exatamente aquele artigo.
        /// </summary>
        /// <param name="uuid">Identificador único da notícia</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>Detalhes completos da notícia</returns>
        public async Task<object> GetArticleAsync(string uuid, CancellationToken ct)
        {
            return await _marketNewsIntegration.GetNewsByUuidAsync(uuid, ct);
        }

        /// <summary>
        /// Busca notícias similares com base em um artigo específico.
        /// 
        /// A API retorna conteúdos relacionados ao UUID informado,
        /// podendo ser útil para análise de contexto ou reforço de tendência.
        /// </summary>
        /// <param name="uuid">UUID da notícia base</param>
        /// <param name="language">Idioma das notícias (default: en)</param>
        /// <param name="publishedOn">Data das notícias (formato yyyy-MM-dd)</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>Lista de notícias similares</returns>
        public async Task<object> GetSimilarAsync(
            string uuid,
            string? language,
            string? publishedOn,
            CancellationToken ct)
        {
            return await _marketNewsIntegration.GetSimilarNewsAsync(
                uuid,
                language ?? "en",
                publishedOn,
                ct);
        }
    }
}