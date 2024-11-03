using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Domain;

namespace CurrencyIngestion.API.Payload
{
    public record Result(
        Guid ExchangeIdentification,
        decimal Amount,
        OperationType OperationTye,
        decimal Price,
        IEnumerable<ExchangeOperation> CalculationItens);
}