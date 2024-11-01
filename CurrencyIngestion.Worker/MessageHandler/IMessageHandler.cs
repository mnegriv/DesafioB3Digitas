using CurrencyIngestion.Model;

namespace CurrencyIngestion.Worker.MessageHandler
{
    public interface IMessageHandler
    {
        Task Handle(OrderBook orderBook, string messageReceived);
    }
}