using CurrencyIngestion.Domain;

namespace CurrencyIngestion.Worker.MessageHandler
{
    public interface IBitstampMessageHandler
    {
        Task<CurrencySummary> Handle(string messageReceived);
    }
}