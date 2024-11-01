using CurrencyIngestion.Model;

namespace CurrencyIngestion.Worker.MessageHandler
{
    public interface IMessageHandlerFactory
    {
        IMessageHandler Create(OrderBook orderBook);
    }
}