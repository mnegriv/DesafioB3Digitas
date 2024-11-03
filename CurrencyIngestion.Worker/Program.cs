using Azure.Identity;
using CurrencyIngestion.Common;
using CurrencyIngestion.Data;
using CurrencyIngestion.Service;
using CurrencyIngestion.Worker.MessageHandler;
using Microsoft.Azure.Cosmos;

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
                    services.AddSingleton<ICurrencySummaryCalculator, CurrencySummaryCalculator>();
                    services.AddSingleton<IBitstampMessageHandler, BitstampMessageHandler>();
                    services.AddSingleton<ILiveOrderBookPoolingAdapter, LiveOrderBookPoolingAdapter>();
                })
                .Build();

            host.Run();
        }
    }
}