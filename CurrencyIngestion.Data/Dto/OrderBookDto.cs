using CurrencyIngestion.Domain;

namespace CurrencyIngestion.Data.Dto
{
    public record OrderBookDto(string? id, OrderDataDto data, string channel, string @event)
    {
        public static explicit operator OrderBookDto(OrderBook orderBook)
        {
            return new OrderBookDto(orderBook.Id, (OrderDataDto)orderBook.Data, orderBook.Channel, orderBook.Event);
        }
    }

    public record OrderDataDto(string? timestamp, List<List<string>> bids, List<List<string>> asks)
    {
        public static explicit operator OrderDataDto(OrderData data)
        {
            return new OrderDataDto(data.Timestamp, data.Bids, data.Asks);
        }
    };
}