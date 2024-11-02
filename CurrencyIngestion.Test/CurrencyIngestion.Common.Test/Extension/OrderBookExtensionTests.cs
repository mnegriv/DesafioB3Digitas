using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Common.Extension;
using CurrencyIngestion.Model;

namespace CurrencyIngestion.Test.CurrencyIngestion.Common.Test.Extension
{
    public class OrderBookExtensionTests
    {
        [Fact]
        [Trait("ExchangeOperationConvertion", "Ask")]
        public void Given_OrderBookData_When_ConvertToAskOperations_Then_ConvertedDataIsCorrect()
        {
            OrderBook orderBook = new()
            {
                Data = new()
                {
                    Asks = new List<List<string>>
                    {
                        new() { "100.5", "0.1" }
                    }
                }
            };

            var converted = orderBook.ToAskOperations(CurrencyPair.BTCUSD).ToList();

            Assert.IsAssignableFrom<IEnumerable<Operation>>(converted);
            Assert.NotEmpty(converted);
            Assert.Equal("BTCUSD", converted.First().Currency);
            Assert.Equal(100.5m, converted.First().Price);
            Assert.Equal(0.1m, converted.First().Amount);
        }

        [Fact]
        [Trait("ExchangeOperationConvertion", "Ask")]
        public void Given_NullOrderBookData_When_ConvertToAskOperations_Then_ConvertedDataIsEmpty()
        {
            OrderBook orderBook = new()
            {
                Data = null
            };

            var converted = orderBook.ToAskOperations(CurrencyPair.BTCUSD).ToList();

            Assert.IsAssignableFrom<IEnumerable<Operation>>(converted);
            Assert.Empty(converted);
        }

        [Fact]
        [Trait("ExchangeOperationConvertion", "Ask")]
        public void Given_OrderBookDataWithInvalidDecimalValue_When_ConvertToAskOperations_Then_ConvertedDataValuesAreZero()
        {
            OrderBook orderBook = new()
            {
                Data = new()
                {
                    Asks = new List<List<string>>
                    {
                        new() { "invalid", "invalid" }
                    }
                }
            };

            var converted = orderBook.ToAskOperations(CurrencyPair.BTCUSD).ToList();

            Assert.IsAssignableFrom<IEnumerable<Operation>>(converted);
            Assert.NotEmpty(converted);
            Assert.Equal("BTCUSD", converted.First().Currency);
            Assert.Equal(0m, converted.First().Price);
            Assert.Equal(0m, converted.First().Amount);
        }

        [Fact]
        [Trait("ExchangeOperationConvertion", "Bid")]
        public void Given_OrderBookData_When_ConvertToBidOperations_Then_ConvertedDataIsCorrect()
        {
            OrderBook orderBook = new()
            {
                Data = new()
                {
                    Bids = new List<List<string>>
                    {
                        new() { "700.5", "0.5" }
                    }
                }
            };

            var converted = orderBook.ToBidOperations(CurrencyPair.BTCUSD).ToList();

            Assert.IsAssignableFrom<IEnumerable<Operation>>(converted);
            Assert.NotEmpty(converted);
            Assert.Equal("BTCUSD", converted.First().Currency);
            Assert.Equal(700.5m, converted.First().Price);
            Assert.Equal(0.5m, converted.First().Amount);
        }

        [Fact]
        [Trait("ExchangeOperationConvertion", "Bid")]
        public void Given_NullOrderBookData_When_ConvertToBidOperations_Then_ConvertedDataIsEmpty()
        {
            OrderBook orderBook = new()
            {
                Data = null
            };

            var converted = orderBook.ToBidOperations(CurrencyPair.BTCUSD).ToList();

            Assert.IsAssignableFrom<IEnumerable<Operation>>(converted);
            Assert.Empty(converted);
        }

        [Fact]
        [Trait("ExchangeOperationConvertion", "Bid")]
        public void Given_OrderBookDataWithInvalidDecimalValue_When_ConvertToBidOperations_Then_ConvertedDataValuesAreZero()
        {
            OrderBook orderBook = new()
            {
                Data = new()
                {
                    Bids = new List<List<string>>
                    {
                        new() { "invalid", "invalid" }
                    }
                }
            };

            var converted = orderBook.ToBidOperations(CurrencyPair.BTCUSD).ToList();

            Assert.IsAssignableFrom<IEnumerable<Operation>>(converted);
            Assert.NotEmpty(converted);
            Assert.Equal("BTCUSD", converted.First().Currency);
            Assert.Equal(0m, converted.First().Price);
            Assert.Equal(0m, converted.First().Amount);
        }
    }
}