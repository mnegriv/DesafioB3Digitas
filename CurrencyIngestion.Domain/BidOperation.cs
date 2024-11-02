using CurrencyIngestion.Common.Enums;

namespace CurrencyIngestion.Domain
{
    public record BidOperation : ExchangeOperation
    {
        public BidOperation(CurrencyPair Currency, decimal Amount, decimal Price)
            : base(Currency, Amount, Price, OperationType.Bid) { }
    }
}