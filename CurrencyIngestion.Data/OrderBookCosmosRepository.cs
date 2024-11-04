using CurrencyIngestion.Data.Dto;
using CurrencyIngestion.Domain;
using Microsoft.Azure.Cosmos;

namespace CurrencyIngestion.Data
{
    public class OrderBookCosmosRepository : IOrderBookRepository
    {
        private const string databaseName = "currencyIngestion";
        private const string containerName = "orderBook";
        private readonly Database database;
        private readonly Container container;

        public OrderBookCosmosRepository(CosmosClient client)
        {
            database = client.GetDatabase(databaseName);
            container = database.GetContainer(containerName);
        }

        public async Task Save(OrderBook orderBook)
        {
            orderBook.Id = Guid.NewGuid().ToString();
            OrderBookDto dto = (OrderBookDto)orderBook;
            OrderBookDto createdItem = await container.CreateItemAsync(
                item: dto,
                partitionKey: new PartitionKey(orderBook.Channel)
                );
        }

        public async Task<IEnumerable<OrderBook>> GetAll(string channelName)
        {
            string queryText = $"SELECT * FROM orderBook WHERE orderBook.channel = '{channelName}'";
            var results = await QueryMany<OrderBook>(queryText);

            return results;
        }

        public async Task<OrderBook> GetLatest(string channelName)
        {
            string queryText = $@"
                SELECT * FROM orderBook 
                WHERE orderBook.channel = '{channelName}' 
                ORDER BY orderBook._ts DESC OFFSET 0 LIMIT 1";

            var result = await QueryFirstOrDefault<OrderBook>(queryText);

            return result ?? new OrderBook();
        }

        private async Task<List<T>> QueryMany<T>(string queryText)
        {
            using FeedIterator<T> feed = container.GetItemQueryIterator<T>(
                queryText: queryText
            );

            List<T> items = new();

            while (feed.HasMoreResults)
            {
                FeedResponse<T> response = await feed.ReadNextAsync();

                foreach (T item in response)
                    items.Add(item);
            }

            return items;
        }

        private async Task<T?> QueryFirstOrDefault<T>(string queryText) where T : class
        {
            using FeedIterator<T> feed = container.GetItemQueryIterator<T>(
                queryText: queryText
            );

            while (feed.HasMoreResults)
            {
                FeedResponse<T> response = await feed.ReadNextAsync();

                return response.FirstOrDefault();
            }

            return null;
        }
    }
}