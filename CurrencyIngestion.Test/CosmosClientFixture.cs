using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace CurrencyIngestion.Test
{
    public class CosmosClientFixture
    {
        private readonly IConfiguration configuration;

        public CosmosClientFixture()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.Test.json");

            this.configuration = builder.Build();
        }

        public CosmosClient CreateClient(string connectionName)
        {
            string? connString = configuration.GetConnectionString(connectionName)
                ?? throw new KeyNotFoundException($"The connection string was not informed for '{connectionName}'");

            return new CosmosClient(connString);
        }
    }
}