using CurrencyIngestion.Domain;
using CurrencyIngestion.Service;

namespace CurrencyIngestion.Test.CurrencyIngestion.Service.Test
{
    public class CurrencySummaryCalculatorTests
    {
        [Fact]
        public void Given_OrderBookWithBidsAndAsks_When_CalculateSummary_Then_CurrentPricesAndAverageAreCorrect()
        {
            CurrencySummaryCalculator service = new();

            OrderBook orderBookNow = new()
            {
                Channel = "eth_test",
                Data = new OrderData
                {
                    Timestamp = "999999",
                    Bids = new List<List<string>>
                    {
                        new() { "72926.1", "0.00000000" },
                        new() { "72927.1", "0.00000000" },
                        new() { "72922.1", "0.00000000" },
                        new() { "72906.1", "0.00000000" }
                    },
                    Asks = new List<List<string>>
                    {
                        new() { "72936.1", "0.00000000" },
                        new() { "72937.1", "0.00000000" },
                        new() { "72932.1", "0.00000000" },
                        new() { "72916.1", "0.00000000" }
                    },
                }
            };

            var summary = service.CalculateSummary(orderBookNow, null, null);

            var average = new List<decimal> 
            { 
                72926.1m, 72927.1m, 72922.1m, 72906.1m,
                72936.1m, 72937.1m, 72932.1m, 72916.1m
            }.Average();

            Assert.Equal(72937.1m, summary.HighestPrice);
            Assert.Equal(72906.1m, summary.LowestPrice);
            Assert.Equal(average, summary.AveragePriceCurrent);
        }

        [Fact]
        public void Given_CurrentAndPreviousOrderBook_When_CalculateSummary_Then_AveragePriceWithPreviousIsCorrect()
        {
            CurrencySummaryCalculator service = new();

            OrderBook orderBookNow = new()
            {
                Channel = "eth_test",
                Data = new OrderData
                {
                    Timestamp = "999999",
                    Bids = new List<List<string>>
                    {
                        new() { "72926.0", "0.00000000" },
                        new() { "72927.0", "0.00000000" },
                        new() { "72922.0", "0.00000000" },
                        new() { "72906.0", "0.00000000" }
                    }
                }
            };

            OrderBook orderBookPrevious = new()
            {
                Channel = "eth_test",
                Data = new OrderData
                {
                    Timestamp = "999999",
                    Bids = new List<List<string>>
                    {
                        new() { "72916.0", "0.00000000" },
                        new() { "72917.0", "0.00000000" },
                        new() { "72912.0", "0.00000000" },
                        new() { "72896.0", "0.00000000" }
                    }
                }
            };

            var summary = service.CalculateSummary(orderBookNow, orderBookPrevious, null);

            var average = new List<decimal>
                { 72926.0m, 72927.0m, 72922.0m, 72906.0m, 72916.0m, 72917.0m, 72912.0m, 72896.0m }
            .Average();

            Assert.Equal(average, summary.AveragePriceWithPrevious);
        }

        [Fact]
        public void Given_CumulativeOrderBooks_When_CalculateSummary_Then_AveragePriceCumulativeIsCorrect()
        {
            CurrencySummaryCalculator service = new();

            OrderBook orderBook1 = new()
            {
                Channel = "eth_test",
                Data = new OrderData
                {
                    Bids = new List<List<string>>
                    {
                        new() { "10.1", "0.00000000" },
                        new() { "11.1", "0.00000000" },
                        new() { "12.1", "0.00000000" },
                        new() { "13.1", "0.00000000" }
                    }
                }
            };

            OrderBook orderBook2 = new()
            {
                Channel = "eth_test",
                Data = new OrderData
                {
                    Bids = new List<List<string>>
                    {
                        new() { "20.2", "0.00000000" },
                        new() { "21.2", "0.00000000" },
                        new() { "22.2", "0.00000000" },
                        new() { "23.2", "0.00000000" }
                    }
                }
            };

            var orders = new OrderBook[] { orderBook1, orderBook2 };

            var summary = service.CalculateSummary(null, null, orders);

            var average = new List<decimal> { 10.1m, 11.1m, 12.1m, 13.1m, 20.2m, 21.2m, 22.2m, 23.2m }.Average();

            Assert.Equal(average, summary.AveragePriceCumulative);
        }

        [Fact]
        public void Given_NullOrderBookData_When_CalculateSummary_Then_AllValuesAreZero()
        {
            CurrencySummaryCalculator service = new();

            OrderBook orderBookNow = new()
            {
                Channel = "eth_test",
                Data = null
            };

            var summary = service.CalculateSummary(orderBookNow, null, null);

            Assert.Equal(0m, summary.HighestPrice);
            Assert.Equal(0m, summary.LowestPrice);
            Assert.Equal(0m, summary.AveragePriceCurrent);
            Assert.Equal(0m, summary.AveragePriceWithPrevious);
            Assert.Equal(0m, summary.AveragePriceCumulative);
        }

        [Fact]
        public void Given_OrderBookDataWithInvalidValues_When_CalculateSummary_Then_AllValuesAreZero()
        {
            CurrencySummaryCalculator service = new();

            OrderBook orderBookNow = new()
            {
                Channel = "btc_test",
                Data = new OrderData
                {
                    Timestamp = "999999",
                    Bids = new List<List<string>>
                    {
                        new() { "invalid", "invalid" }
                    }
                }
            };

            var summary = service.CalculateSummary(orderBookNow, null, null);

            Assert.Equal(0m, summary.HighestPrice);
            Assert.Equal(0m, summary.LowestPrice);
            Assert.Equal(0m, summary.AveragePriceCurrent);
            Assert.Equal(0m, summary.AveragePriceWithPrevious);
            Assert.Equal(0m, summary.AveragePriceCumulative);
        }
    }
}