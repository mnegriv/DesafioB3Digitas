using CurrencyIngestion.Common.Enums;

namespace CurrencyIngestion.Domain
{
    public abstract record ExchangeOperation(CurrencyPair Currency, decimal Amount, decimal Price, OperationType Type);
}