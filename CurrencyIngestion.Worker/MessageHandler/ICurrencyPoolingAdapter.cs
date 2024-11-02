namespace CurrencyIngestion.Worker.MessageHandler
{
    public interface ICurrencyPoolingAdapter
    {
        Task Pool(CancellationToken stoppingToken);
    }
}