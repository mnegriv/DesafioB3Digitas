using CurrencyIngestion.Common.Enums;

namespace CurrencyIngestion.Data
{
    public interface ICurrencyRepository
    {
        Task Save(string orderBook, CurrencyPair currency);

        Task<IEnumerable<string>> GetAll(CurrencyPair currency);

        Task<string> GetLatest(CurrencyPair currency);
    }
}