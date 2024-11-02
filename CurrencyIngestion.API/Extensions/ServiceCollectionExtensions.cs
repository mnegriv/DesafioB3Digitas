using CurrencyIngestion.Common;
using CurrencyIngestion.Data;
using CurrencyIngestion.Service;

namespace CurrencyIngestion.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IExchangeSimulationService, ExchangeSimulationService>();
            services.AddSingleton<ICurrencyRepository>(c =>
            {
                var config = c.GetRequiredService<IConfiguration>();
                string connString = config.GetConnectionString(Constants.CURRENCY_CONNECTIONSTRING_NAME)
                    ?? throw new KeyNotFoundException($"The connection string was not informed for '{Constants.CURRENCY_CONNECTIONSTRING_NAME}'");

                return new CurrencySQLServerRepository(connString);
            });
            services.AddSingleton<IExchangeSimulationRepository>(c =>
            {
                var config = c.GetRequiredService<IConfiguration>();
                string connString = config.GetConnectionString(Constants.EXCHANGESIMULATION_CONNECTIONSTRING_NAME)
                    ?? throw new KeyNotFoundException($"The connection string was not informed for '{Constants.EXCHANGESIMULATION_CONNECTIONSTRING_NAME}'");

                return new ExchangeSimulationSQLServerRepository(connString);
            });
        }
    }
}