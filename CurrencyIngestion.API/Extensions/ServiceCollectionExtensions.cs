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
                //TODO: pegar da config
                string connString = "Server=localhost\\SQLEXPRESS01;Database=CurrencyIngestion;Trusted_Connection=True;";
                return new CurrencyRepositorySQLServer(connString);
            });
        }
    }
}