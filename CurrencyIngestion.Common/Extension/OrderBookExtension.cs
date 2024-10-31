using CurrencyIngestion.Model;
using System.Globalization;

namespace CurrencyIngestion.Common.Extension
{
    public static class OrderBookExtension
    {
        public static IEnumerable<Operation> ToAskOperations(this OrderBook orderBook)
        {
            var orderBookAsks = orderBook?.Data?.Asks;

            foreach (var data in orderBookAsks ?? new List<List<string>>())
            {
                Operation operation = new()
                {
                    Currency = "BTC",
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