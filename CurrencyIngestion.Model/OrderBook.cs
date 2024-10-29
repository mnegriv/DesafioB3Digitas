namespace CurrencyIngestion.Model
{
    public record OrderBook 
    { 
        public OrderData? Data { get; init; } 
        public string? Channel { get; init; } 
        public string? Event { get; init; } 
    }
    public record OrderData 
    {
        public string? Timestamp { get; init; }
        public DateTimeOffset Time => DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(Timestamp ?? "0"));
        public string TimeStr => Time.ToLocalTime().ToString();
        public List<List<string>>? Bids { get; init; }
        public List<List<string>>? Asks { get; init; }
    }
}