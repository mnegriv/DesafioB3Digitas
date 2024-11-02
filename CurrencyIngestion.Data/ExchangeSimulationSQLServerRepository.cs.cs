using CurrencyIngestion.Domain;
using Dapper;
using System.Data.SqlClient;
using System.Text.Json;

namespace CurrencyIngestion.Data
{
    public class ExchangeSimulationSQLServerRepository : IExchangeSimulationRepository
    {
        private readonly string connString;

        public ExchangeSimulationSQLServerRepository(string connString)
        {
            this.connString = connString;
        }

        public async Task Save(ExchangeSimulation exchangeSimulation)
        {
            using var connection = new SqlConnection(connString);

            connection.Open();

            string command = $"INSERT INTO SimulationLog (Content) OUTPUT inserted.Identifier VALUES (@exchangeSimulation)";

            string serialized = JsonSerializer.Serialize(exchangeSimulation);

            await connection.ExecuteScalarAsync(command, new { exchangeSimulation = serialized });
        }
    }
}