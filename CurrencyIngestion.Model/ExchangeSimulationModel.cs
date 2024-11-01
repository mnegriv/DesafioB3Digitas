using System.Runtime.CompilerServices;

namespace CurrencyIngestion.Model
{
    public class ExchangeSimulationModel
    {
        public ExchangeSimulationModel(string currency, string type)
        {
            Currency = currency;
            Type = type;
        }

        public string Currency { get; set; }
        public string Type { get; set; }
        public List<Operation> Operations { get; internal set; } = new List<Operation>();
        public decimal TotalAmount => Operations.Sum(o => o.Amount);
        public decimal TotalPrice { get; internal set; }

        public void AddOperation(Operation op) => Operations.Add(op);
        public void IncrementTotalPrice(decimal price, decimal amount) => TotalPrice += (price * amount);
    }
}