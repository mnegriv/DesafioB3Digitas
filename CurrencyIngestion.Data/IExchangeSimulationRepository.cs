using CurrencyIngestion.Model;

namespace CurrencyIngestion.Data
{
    public interface IExchangeSimulationRepository
    {
        Task Save(ExchangeSimulationModel exchangeSimulationModel);
    }
}