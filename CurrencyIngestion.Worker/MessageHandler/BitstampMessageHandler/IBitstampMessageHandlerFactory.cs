using CurrencyIngestion.Model;

namespace CurrencyIngestion.Worker.MessageHandler.BitstampMessageHandler
{
    public interface IBitstampMessageHandlerFactory
    {
        IBitstampMessageHandler Create(OrderBook orderBook);
    }
}