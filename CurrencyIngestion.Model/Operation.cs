namespace CurrencyIngestion.Model
{
    public class Operation
    {
        public string Currency { get; set; } = "[not defined]";
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
    }
}