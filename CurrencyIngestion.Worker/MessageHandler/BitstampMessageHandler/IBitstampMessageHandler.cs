using CurrencyIngestion.Model;

namespace CurrencyIngestion.Worker.MessageHandler.BitstampMessageHandler
{
    public interface IBitstampMessageHandler
    {
        Task Handle(OrderBook orderBook, string messageReceived);
    }
}