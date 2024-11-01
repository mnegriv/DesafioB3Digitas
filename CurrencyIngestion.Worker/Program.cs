using CurrencyIngestion.Data;
using CurrencyIngestion.Service;
using CurrencyIngestion.Worker.MessageHandler;

namespace CurrencyIngestion.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();
                    services.AddMemoryCache();
                    services.AddSingleton<ICurrencyRepository>(c =>
                    {
                        //TODO: pegar da config
                        string connString = "Server=localhost\\SQLEXPRESS01;Database=CurrencyIngestion;Trusted_Connection=True;";
                        return new CurrencySQLServerRepository(connString);
                    });
                    services.AddSingleton<ICurrencySummaryCalculator, CurrencySummaryCalculator>();
                    services.AddSingleton<IMessageHandlerFactory, MessageHandlerFactory>();
                })
                .Build();

            host.Run();
        }
    }
}