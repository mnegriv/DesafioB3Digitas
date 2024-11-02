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
                        //TODO: pegar da config
                        string connString = "Server=localhost\\SQLEXPRESS01;Database=CurrencyIngestion;Trusted_Connection=True;";
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