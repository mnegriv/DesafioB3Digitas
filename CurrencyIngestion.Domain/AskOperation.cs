using CurrencyIngestion.Common.Enums;

namespace CurrencyIngestion.Domain
{
    public record AskOperation : ExchangeOperation
    {
        public AskOperation(CurrencyPair Currency, decimal Amount, decimal Price)
            : base(Currency, Amount, Price, OperationType.Ask) { }
    }
}