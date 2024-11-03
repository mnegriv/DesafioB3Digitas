using System.Text.Json;

namespace CurrencyIngestion.Domain
{
    public record OrderBook
    {
        public string? Id { get; set; }

        public OrderData Data { get; init; } = new();

        public string Channel { get; init; } = string.Empty;

        public string Event { get; init; } = string.Empty;

        public static OrderBook FromJson(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<OrderBook>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                }) ?? new OrderBook();
            }
            catch (Exception)
            {
                return new OrderBook();
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