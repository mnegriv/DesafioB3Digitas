using Azure.Identity;
using CurrencyIngestion.Common;
using CurrencyIngestion.Data;
using CurrencyIngestion.Service;
using CurrencyIngestion.Worker.MessageHandler.BitstampMessageHandler;
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
                    services.AddSingleton<IOrderBookRepository>(c =>
                    {
                        var config = c.GetRequiredService<IConfiguration>();
                        string connString = config.GetConnectionString(Constants.CURRENCY_CONNECTIONSTRING_NAME)
                            ?? throw new KeyNotFoundException($"The connection string was not informed for '{Constants.CURRENCY_CONNECTIONSTRING_NAME}'"); ;

                        //TODO colocar switch
                        //var config = c.GetRequiredService<IConfiguration>();
                        //string connString = config.GetConnectionString(Constants.CURRENCY_CONNECTIONSTRING_NAME)
                        //    ?? throw new KeyNotFoundException($"The connection string was not informed for '{Constants.CURRENCY_CONNECTIONSTRING_NAME}'");

                        //return new OrderBookSQLServerRepository(connString);

                        CosmosClient cosmosClient = new(connString);
                        return new OrderBookCosmosRepository(cosmosClient);
                    });
                    services.AddSingleton<ICurrencySummaryCalculator, CurrencySummaryCalculator>();
                    services.AddSingleton<IBitstampMessageHandlerFactory, BitstampMessageHandlerFactory>();
                    services.AddSingleton<ILiveOrderBookPoolingAdapter, LiveOrderBookPoolingAdapter>();
                })
                .Build();

            host.Run();
        }
    }
}