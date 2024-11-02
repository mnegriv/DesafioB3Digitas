using CurrencyIngestion.Domain;

namespace CurrencyIngestion.Worker.MessageHandler.BitstampMessageHandler
{
    public interface IBitstampMessageHandlerFactory
    {
        IBitstampMessageHandler Create(string message);
    }
}