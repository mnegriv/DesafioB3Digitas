using System.Text.Json.Serialization;

namespace CurrencyIngestion.Worker.Model
{
    public record ChannelData
    {
        public ChannelData(string? name)
        {
            Name = name;
        }

        [JsonPropertyName("channel")]
        public string? Name { get; init; }
    }
}