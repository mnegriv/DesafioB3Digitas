namespace CurrencyIngestion.Worker
{
    public class WorkerService : BackgroundService
    {
        private readonly ILogger<WorkerService> logger;
        private readonly ILiveOrderBookPoolingAdapter currencyPoolingAdapter;

        public WorkerService(
            ILogger<WorkerService> logger,
            ILiveOrderBookPoolingAdapter currencyPoolingAdapter)
        {
            this.logger = logger;
            this.currencyPoolingAdapter = currencyPoolingAdapter;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await this.currencyPoolingAdapter.Pool(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "An error has occoured while executing the worker.");
            }
        }
    }
}