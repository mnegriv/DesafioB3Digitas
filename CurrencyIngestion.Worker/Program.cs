using CurrencyIngestion.Data;
using CurrencyIngestion.Service;

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
                    services.AddSingleton<ICurrencyRepository, FakeCurrencyRepo>();
                    services.AddSingleton<ICurrencySummaryCalculator, CurrencySummaryCalculator>();
                })
                .Build();

            host.Run();
        }
    }
}