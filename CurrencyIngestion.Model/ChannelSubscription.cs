using System.Text.Json.Serialization;

namespace CurrencyIngestion.Model
{
    public record ChannelSubscription
    {
        [JsonPropertyName("event")]
        public string? Event => "bts:subscribe";

        [JsonPropertyName("data")]
        public ChannelData? Data { get; set; }
    }
}