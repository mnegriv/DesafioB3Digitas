using CurrencyIngestion.Data.Dto;
using CurrencyIngestion.Domain;
using Microsoft.Azure.Cosmos;

namespace CurrencyIngestion.Data
{
    public class ExchangeSimulationCosmosRepository : IExchangeSimulationRepository
    {
        private const string databaseName = "currencyIngestion";
        private const string containerName = "simulationLog";
        private readonly Database database;
        private readonly Container container;

        public ExchangeSimulationCosmosRepository(CosmosClient client)
        {
            database = client.GetDatabase(databaseName);
            container = database.GetContainer(containerName);
        }

        public async Task Save(ExchangeSimulation exchangeSimulationModel)
        {
            ExchangeSimulationDto dto = (ExchangeSimulationDto)exchangeSimulationModel;

            ExchangeSimulationDto createdItem = await container.CreateItemAsync(
                item: dto,
                partitionKey: new PartitionKey(dto.currency)
                );
        }
    }
}