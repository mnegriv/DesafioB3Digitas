using CurrencyIngestion.Common;
using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Common.Extensions;
using CurrencyIngestion.Data;
using CurrencyIngestion.Domain;
using Microsoft.Azure.Cosmos;

namespace CurrencyIngestion.Test.CurrencyIngestion.Data.Test
{
    [Trait("Integration", "Repository")]
    public class OrderBookCosmosRepositoryTests : IClassFixture<CosmosClientFixture>
    {
        private readonly CosmosClientFixture cosmosClientFixture;

        public OrderBookCosmosRepositoryTests(CosmosClientFixture cosmosClientFixture)
        {
            this.cosmosClientFixture = cosmosClientFixture;
        }

        [Fact]
        public async Task Given_OrderBookRepository_When_GetAllOrderBooks_Then_QueryIsPerformed()
        {
            OrderBookCosmosRepository repository = CreateRepository();

            var result = await repository.GetAll(CurrencyPair.BTCUSD.ToOrderBookChannel());

            Assert.NotNull(result);
        }

        [Fact]
        public async Task Given_OrderBookRepository_When_GetLatestOrderBook_Then_QueryIsPerformed()
        {
            OrderBookCosmosRepository repository = CreateRepository();

            var result = await repository.GetLatest(CurrencyPair.BTCUSD.ToOrderBookChannel());

            Assert.IsType<OrderBook>(result);
        }

        private OrderBookCosmosRepository CreateRepository()
        {
            CosmosClient cosmosClient = cosmosClientFixture.CreateClient(Constants.CURRENCY_CONNECTIONSTRING_NAME);
            return new OrderBookCosmosRepository(cosmosClient);
        }
    }
}