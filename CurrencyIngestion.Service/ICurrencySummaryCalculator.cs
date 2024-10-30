using CurrencyIngestion.Model;

namespace CurrencyIngestion.Service
{
    public interface ICurrencySummaryCalculator
    {
        CurrencySummary CalculateSummary(OrderBook? orderBookCurrent, OrderBook? orderBookPrevious, IEnumerable<OrderBook?>? cumulativeOrders);
    }
}