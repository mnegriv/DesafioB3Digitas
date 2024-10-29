using System.Text.Json.Serialization;

namespace CurrencyIngestion.Model
{
    public enum TradeType
    {
        Buy = 0,
        Sell = 1,
        None
    }

    public record LiveTicker
    {
        public int Id { get; init; }

        public string? Timestamp { get; init; }

        public DateTimeOffset Time => DateTimeOffset.FromUnixTimeSeconds(long.Parse(Timestamp ?? "0"));

        public string TimeStr => Time.ToLocalTime().ToString();

        public decimal Amount { get; init; }

        [JsonPropertyName("amount_str")]
        public string? AmountStr { get; init; }

        public decimal Price { get; init; }

        [JsonPropertyName("price_str")]
        public string? PriceStr { get; init; }

        public TradeType Type { get; init; }

        public string? Microtimestamp { get; init; }

        [JsonPropertyName("buy_order_id")]
        public long BuyOrderId { get; init; }

        [JsonPropertyName("sell_order_id")]
        public long SellOrderId { get; init; }
    }
}