using System.Text.Json;

namespace CurrencyIngestion.Model
{
    public record OrderBook 
    { 
        public OrderData? Data { get; init; }
        public string Channel { get; init; } = string.Empty;
        public string Event { get; init; } = string.Empty;

        public static OrderBook? FromJson(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<OrderBook>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public record OrderData 
    {
        public string? Timestamp { get; init; }
        public List<List<string>> Bids { get; init; } = new List<List<string>>();
        public List<List<string>> Asks { get; init; } = new List<List<string>>();
    }
}