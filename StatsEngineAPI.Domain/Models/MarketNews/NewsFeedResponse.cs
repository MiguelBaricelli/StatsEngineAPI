using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StatsEngineAPI.Domain.Models.MarketNews
{
    public class NewsFeedResponse
    {
        public string Items { get; set; }
        public string SentimentScoreDefinition { get; set; }
        public string RelevanceScoreDefinition { get; set; }
        public List<FeedItem> Feed { get; set; } = new();
    }

    public class FeedItem
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string TimePublished { get; set; }
        public List<string> Authors { get; set; } = new();
        public string Summary { get; set; }
        public string BannerImage { get; set; }
        public string Source { get; set; }
        public string CategoryWithinSource { get; set; }
        public string SourceDomain { get; set; }
        public List<InTopic> Topics { get; set; } = new();
        public decimal OverallSentimentScore { get; set; }   // número no JSON ✅
        public string OverallSentimentLabel { get; set; }
        public List<TickerSentiment> TickerSentiment { get; set; } = new();
    }

    public class InTopic
    {
        [JsonPropertyName("topic")]          // JSON usa "topic", classe usa "Topic" — conflito sem isso
        public string TopicName { get; set; }

        [JsonPropertyName("relevance_score")]
        public string RelevanceScore { get; set; }   // ⚠️ string no JSON: "0.948813"
    }

    public class TickerSentiment
    {
        public string Ticker { get; set; }

        [JsonPropertyName("relevance_score")]
        public string RelevanceScore { get; set; }           // ⚠️ string no JSON: "1.000000"

        [JsonPropertyName("ticker_sentiment_score")]
        public string TickerSentimentScore { get; set; }     // ⚠️ string no JSON: "0.420039"

        public string TickerSentimentLabel { get; set; }
    }
}