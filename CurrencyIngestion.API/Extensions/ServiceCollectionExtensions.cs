using CurrencyIngestion.Common;
using CurrencyIngestion.Data;
using CurrencyIngestion.Service;
using Microsoft.Azure.Cosmos;

namespace CurrencyIngestion.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IExchangeSimulationService, ExchangeSimulationService>();
            services.AddSingleton(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                string connString = config.GetConnectionString(Constants.CURRENCY_CONNECTIONSTRING_NAME)
                    ?? throw new KeyNotFoundException($"The connection string was not informed for '{Constants.CURRENCY_CONNECTIONSTRING_NAME}'"); ;

                CosmosClient cosmosClient = new(connString);

                return cosmosClient;
            });
            services.AddSingleton<IOrderBookRepository>(provider =>
            {
                var cosmosClient = provider.GetRequiredService<CosmosClient>();
                return new OrderBookCosmosRepository(cosmosClient);
            });
            services.AddSingleton<IExchangeSimulationRepository>(provider =>
            {
                var cosmosClient = provider.GetRequiredService<CosmosClient>();
                return new ExchangeSimulationCosmosRepository(cosmosClient);
            });
        }
    }
}