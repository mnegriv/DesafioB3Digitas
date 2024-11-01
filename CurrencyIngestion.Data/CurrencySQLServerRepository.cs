using CurrencyIngestion.Common.Enums;
using Dapper;
using System.Data.SqlClient;

namespace CurrencyIngestion.Data
{
    public class CurrencySQLServerRepository : ICurrencyRepository
    {
        private readonly string connString;

        public CurrencySQLServerRepository(string connString)
        {
            this.connString = connString ?? throw new ArgumentNullException(nameof(connString));
        }

        public async Task Save(string orderBook, CurrencyPair currency)
        {
            string schema = GetSchema(currency);

            using var connection = new SqlConnection(connString);

            connection.Open();

            string command = $"INSERT INTO {schema}.OrderBook (Content) OUTPUT inserted.Identifier VALUES (@orderBook)";

            await connection.ExecuteScalarAsync(command, new { orderBook });
        }

        public async Task<IEnumerable<string>> GetAll(CurrencyPair currency)
        {
            string schema = GetSchema(currency);

            using var connection = new SqlConnection(connString);

            connection.Open();

            string command = $"SELECT Content FROM {schema}.OrderBook;";

            return await connection.QueryAsync<string>(command);
        }

        public async Task<string> GetLatest(CurrencyPair currency)
        {
            string schema = GetSchema(currency);

            using var connection = new SqlConnection(connString);

            connection.Open();

            string command = $"SELECT TOP 1 Content FROM {schema}.OrderBook ORDER BY Time Desc;";

            return await connection.QuerySingleAsync<string>(command);
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
    }
}