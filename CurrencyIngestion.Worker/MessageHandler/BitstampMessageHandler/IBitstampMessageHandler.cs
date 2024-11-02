using CurrencyIngestion.Model;

namespace CurrencyIngestion.Worker.MessageHandler.BitstampMessageHandler
{
    public interface IBitstampMessageHandler
    {
        Task<CurrencySummary> Handle(string messageReceived);
    }
}