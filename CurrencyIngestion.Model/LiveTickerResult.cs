using System.Text.Json.Serialization;

namespace CurrencyIngestion.Model
{
    public record LiveTickerResult
    {
        public LiveTicker? Data { get; init; }
        public string? Channel { get; init; }
    }
}