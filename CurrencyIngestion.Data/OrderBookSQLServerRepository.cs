using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Domain;
using Dapper;
using System.Data.SqlClient;
using System.Text.Json;

namespace CurrencyIngestion.Data
{
    public class OrderBookSQLServerRepository : IOrderBookRepository
    {
        private readonly string connString;

        public OrderBookSQLServerRepository(string connString)
        {
            this.connString = connString ?? throw new ArgumentNullException(nameof(connString));
        }

        public async Task Save(OrderBook orderBook, CurrencyPair currency)
        {
            string schema = GetSchema(currency);

            using var connection = new SqlConnection(connString);

            connection.Open();

            string content = JsonSerializer.Serialize(orderBook);

            string command = $"INSERT INTO {schema}.OrderBook (Content) OUTPUT inserted.Identifier VALUES (@content)";

            await connection.ExecuteScalarAsync(command, new { content });
        }

        public async Task<IEnumerable<OrderBook>> GetAll(CurrencyPair currency)
        {
            string schema = GetSchema(currency);

            using var connection = new SqlConnection(connString);

            connection.Open();

            string command = $"SELECT Identifier, Content FROM {schema}.OrderBook;";

            var queryResult = await connection.QueryAsync<(string identifier, string content)>(command);

            return queryResult.Select(ParseOrderBook);
        }

        public async Task<OrderBook?> GetLatest(CurrencyPair currency)
        {
            string schema = GetSchema(currency);

            using var connection = new SqlConnection(connString);

            connection.Open();

            string command = $"SELECT TOP 1 Identifier, Content FROM {schema}.OrderBook ORDER BY Time Desc;";

            var queryResult = await connection.QuerySingleAsync<(string identifier, string content)>(command);

            return ParseOrderBook(queryResult);
        }

        private static string GetSchema(CurrencyPair currency)
        {
            return currency switch
            {
                CurrencyPair.BTCUSD => "btc",
                CurrencyPair.ETHUSD => "eth",
                _ => throw new InvalidOperationException("Invalid currency"),
            };
        }
        private static OrderBook ParseOrderBook((string identifier, string content) result)
        {
            OrderBook ob = OrderBook.FromJson(result.content);
            ob.Id = result.identifier;
            return ob;
        }
    }
}