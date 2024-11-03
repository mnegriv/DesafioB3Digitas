using CurrencyIngestion.Common.Enums;

namespace CurrencyIngestion.Domain
{
    public class ExchangeSimulation
    {
        public ExchangeSimulation(CurrencyPair currency, OperationType type, Guid identification)
        {
            Currency = currency;
            Type = type;
            Identification = identification;
        }

        public Guid Identification { get; set; }
        public CurrencyPair Currency { get; set; }
        public string CurrencyStr => Currency.ToString();
        public OperationType Type { get; set; }
        public string TypeStr => Type.ToString();
        public List<ExchangeOperation> Operations { get; internal set; } = new List<ExchangeOperation>();
        public decimal TotalAmount => Operations.Sum(o => o.Amount);
        public decimal TotalPrice { get; internal set; }

        public void AddOperation(ExchangeOperation op) => Operations.Add(op);
        public void IncrementTotalPrice(decimal price, decimal amount) => TotalPrice += price * amount;
    }
}