namespace CurrencyIngestion.Data
{
    public interface ICurrencyRepository
    {
        Task Save(string orderBook);

        Task<IEnumerable<string>> GetAll();
    }
}