namespace CurrencyIngestion.Worker
{
    public interface ILiveOrderBookPoolingAdapter
    {
        Task Pool(CancellationToken stoppingToken);
    }
}