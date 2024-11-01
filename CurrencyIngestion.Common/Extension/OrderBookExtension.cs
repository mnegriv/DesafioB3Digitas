using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Model;
using System.Globalization;

namespace CurrencyIngestion.Common.Extension
{
    public static class OrderBookExtension
    {
        public static IEnumerable<Operation> ToAskOperations(this OrderBook orderBook, CurrencyPair currency)
        {
            var orderBookAsks = orderBook?.Data?.Asks;

            foreach (var data in orderBookAsks ?? new List<List<string>>())
            {
                Operation operation = new()
                {
                    Currency = $"{currency}",
                    Price = ParseDecimal(data[0]),
                    Amount = ParseDecimal(data[1]),
                };

                yield return operation;
            }
        }

        public static IEnumerable<Operation> ToBidOperations(this OrderBook orderBook, CurrencyPair currency)
        {
            var orderBookBids = orderBook?.Data?.Bids;

            foreach (var data in orderBookBids ?? new List<List<string>>())
            {
                Operation operation = new()
                {
                    Currency = $"{currency}",
                    Price = ParseDecimal(data[0]),
                    Amount = ParseDecimal(data[1]),
                };

                yield return operation;
            }
        }

        private static decimal ParseDecimal(string data)
        {
            if (decimal.TryParse(data, CultureInfo.InvariantCulture, out decimal converted))
                return converted;

            return 0m;
        }
    }
}