using Azure.Identity;
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
            services.AddSingleton<IOrderBookRepository>(c =>
            {
                var config = c.GetRequiredService<IConfiguration>();
                string connString = config.GetConnectionString(Constants.CURRENCY_CONNECTIONSTRING_NAME)
                    ?? throw new KeyNotFoundException($"The connection string was not informed for '{Constants.CURRENCY_CONNECTIONSTRING_NAME}'"); ;

                //TODO colocar switch
                //return new OrderBookSQLServerRepository(connString);

                CosmosClient cosmosClient = new(connString);
                return new OrderBookCosmosRepository(cosmosClient);
            });
            services.AddSingleton<IExchangeSimulationRepository>(c =>
            {
                var config = c.GetRequiredService<IConfiguration>();
                string connString = config.GetConnectionString(Constants.EXCHANGESIMULATION_CONNECTIONSTRING_NAME)
                    ?? throw new KeyNotFoundException($"The connection string was not informed for '{Constants.EXCHANGESIMULATION_CONNECTIONSTRING_NAME}'");

                //return new ExchangeSimulationSQLServerRepository(connString);

                CosmosClient cosmosClient = new(connString);
                return new ExchangeSimulationCosmosRepository(cosmosClient);
            });
        }
    }
}