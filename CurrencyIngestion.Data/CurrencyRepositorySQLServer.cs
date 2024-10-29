using Dapper;
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

        public void Save(string orderBook)
        {
            using var connection = new SqlConnection(connString);

            connection.Open();

            string sql = "INSERT INTO Currency VALUES (GETDATE(), @orderBook)";

            connection.ExecuteScalar(sql, new { orderBook });
        }
    }
}