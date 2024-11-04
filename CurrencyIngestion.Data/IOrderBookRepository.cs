using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Domain;

namespace CurrencyIngestion.Data
{
    public interface IOrderBookRepository
    {
        Task Save(OrderBook orderBook);

        Task<IEnumerable<OrderBook>> GetAll(string channelName);

        Task<OrderBook> GetLatest(string channelName);
    }
}