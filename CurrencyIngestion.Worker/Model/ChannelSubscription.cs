using System.Text.Json.Serialization;

namespace CurrencyIngestion.Worker.Model
{
    public record ChannelSubscription
    {
        [JsonPropertyName("event")]
        public string? Event => "bts:subscribe";

        [JsonPropertyName("data")]
        public ChannelData? Data { get; init; }
    }
}