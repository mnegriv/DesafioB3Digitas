using CurrencyIngestion.Domain;

namespace CurrencyIngestion.Service
{
    public class CurrencySummaryCalculator : ICurrencySummaryCalculator
    {
        public CurrencySummary CalculateSummary(OrderBook? orderBookCurrent, OrderBook? orderBookPrevious, IEnumerable<OrderBook?>? cumulativeOrders)
        {
            List<decimal> currentPrices = GetAllPrices(orderBookCurrent);
            List<decimal> previousPrices = GetAllPrices(orderBookPrevious);
            IEnumerable<decimal> allPrices = currentPrices.Concat(previousPrices);

            IEnumerable<decimal> cumulativePrices = GetCumulativePrices(cumulativeOrders)
                .ToList()
                .SelectMany(prices => prices);

            decimal highestCurrentPrice = currentPrices.Any() ? currentPrices.Max() : 0;
            decimal lowestPrice = currentPrices.Any() ? currentPrices.Min() : 0;
            decimal averagePriceCurrent = currentPrices.Any() ? currentPrices.Average() : 0;
            decimal AveragePriceWithPrevious = allPrices.Any() ? allPrices.Average() : 0;
            decimal averagePriceCumulative = cumulativePrices.Any() ? cumulativePrices.Average() : 0;

            var summary = new CurrencySummary(orderBookCurrent?.Channel ?? "[not idenfied]",
                HighestPrice: highestCurrentPrice,
                LowestPrice: lowestPrice,
                AveragePriceCurrent: averagePriceCurrent,
                AveragePriceWithPrevious: AveragePriceWithPrevious,
                AveragePriceCumulative: averagePriceCumulative,
                DateTime.Now);

            return summary;
        }

        private static List<decimal> GetAllPrices(OrderBook? orderBookNow)
        {
            if (orderBookNow is null)
                return new List<decimal>();

            var bidPricesNow = orderBookNow.Data?.Bids.Select(x => x.FirstOrDefault()) ?? new List<string>();
            var asksPricesNow = orderBookNow.Data?.Asks.Select(x => x.FirstOrDefault()) ?? new List<string>();

            List<decimal> allPricesNow = bidPricesNow.Concat(asksPricesNow)
                .ToList().ConvertAll(new Converter<string?, decimal>(StringToDeciamal));

            return allPricesNow;
        }

        private static decimal StringToDeciamal(string? input)
        {
            if (decimal.TryParse(input, System.Globalization.CultureInfo.InvariantCulture, out decimal converted))
                return converted;

            return 0m;
        }

        private static IEnumerable<List<decimal>> GetCumulativePrices(IEnumerable<OrderBook?>? cumulativeOrders)
        {
            if (cumulativeOrders is null)
                yield break;

            foreach (var item in cumulativeOrders)
            {
                yield return GetAllPrices(item);
            }
        }
    }
}