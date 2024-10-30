﻿using Dapper;
using System.Data.SqlClient;

namespace CurrencyIngestion.Data
{
    public class CurrencyRepositorySQLServer : ICurrencyRepository
    {
        private readonly string connString;

        public CurrencyRepositorySQLServer(string connString)
        {
            this.connString = connString ?? throw new ArgumentNullException(nameof(connString));
        }

        public async Task Save(string orderBook)
        {
            using var connection = new SqlConnection(connString);

            connection.Open();

            string sql = "INSERT INTO OrderBook (Content) OUTPUT inserted.Identifier VALUES (@orderBook)";

            await connection.ExecuteScalarAsync(sql, new { orderBook });
        }

        public async Task<IEnumerable<string>> GetAll()
        {
            using var connection = new SqlConnection(connString);

            connection.Open();

            string sql = "SELECT Content FROM OrderBook;";

            return await connection.QueryAsync<string>(sql);
        }
    }
}