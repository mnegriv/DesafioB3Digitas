using System.Runtime.CompilerServices;

namespace CurrencyIngestion.Model
{
    public class ExchangeSimulationModel
    {
        public List<Operation> Operations { get; internal set; } = new List<Operation>();
        public decimal TotalAmount => Operations.Sum(o => o.Amount);
        public decimal TotalPrice { get; internal set; }

        public void AddOperation(Operation op) => Operations.Add(op);
        public void IncrementTotalPrice(decimal price, decimal amount) => TotalPrice += (price * amount);

        public static ExchangeSimulationModel operator +(ExchangeSimulationModel left, ExchangeSimulationModel right)
        {
            return new ExchangeSimulationModel
            {
                Operations = left.Operations.Concat(right.Operations).ToList()
            };
        }
    }
}