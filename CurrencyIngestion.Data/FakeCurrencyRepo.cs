namespace CurrencyIngestion.Data
{
    public class FakeCurrencyRepo : ICurrencyRepository
    {
        public void Save(string orderBook)
        {
            Console.WriteLine("Saved");
        }
    }
}