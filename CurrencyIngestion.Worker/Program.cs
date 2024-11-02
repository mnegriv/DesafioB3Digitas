using CurrencyIngestion.Common;
using CurrencyIngestion.Data;
using CurrencyIngestion.Service;
using CurrencyIngestion.Worker.MessageHandler;
using CurrencyIngestion.Worker.MessageHandler.BitstampMessageHandler;

namespace CurrencyIngestion.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddHostedService<WorkerService>();
                    services.AddMemoryCache();
                    services.AddSingleton<ICurrencyRepository>(c =>
                    {
                        var config = c.GetRequiredService<IConfiguration>();
                        string connString = config.GetConnectionString(Constants.CURRENCY_CONNECTIONSTRING_NAME)
                            ?? throw new KeyNotFoundException($"The connection string was not informed for '{Constants.CURRENCY_CONNECTIONSTRING_NAME}'");

                        return new CurrencySQLServerRepository(connString);
                    });
                    services.AddSingleton<ICurrencySummaryCalculator, CurrencySummaryCalculator>();
                    services.AddSingleton<IBitstampMessageHandlerFactory, BitstampMessageHandlerFactory>();
                    services.AddSingleton<ICurrencyPoolingAdapter, CurrencyPoolingAdapter>();
                })
                .Build();

            host.Run();
        }
    }
}