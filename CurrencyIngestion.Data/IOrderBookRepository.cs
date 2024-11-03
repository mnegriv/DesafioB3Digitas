using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Domain;

namespace CurrencyIngestion.Data
{
    public interface IOrderBookRepository
    {
        Task Save(OrderBook orderBook, CurrencyPair currency);

        Task<IEnumerable<OrderBook>> GetAll(CurrencyPair currency);

        Task<OrderBook?> GetLatest(CurrencyPair currency);
    }
}