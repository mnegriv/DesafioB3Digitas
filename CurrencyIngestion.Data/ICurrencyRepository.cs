namespace CurrencyIngestion.Data
{
    public interface ICurrencyRepository
    {
        void Save(string orderBook);
    }
}