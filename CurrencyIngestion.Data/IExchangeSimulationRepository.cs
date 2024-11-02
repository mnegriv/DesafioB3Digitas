using CurrencyIngestion.Domain;

namespace CurrencyIngestion.Data
{
    public interface IExchangeSimulationRepository
    {
        Task Save(ExchangeSimulation exchangeSimulationModel);
    }
}