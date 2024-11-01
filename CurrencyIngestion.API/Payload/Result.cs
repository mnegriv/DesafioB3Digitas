using CurrencyIngestion.Common.Enums;

namespace CurrencyIngestion.API.Payload
{
    public record Result(
        Guid ExchangeIdentification,
        decimal Amount,
        OperationType OperationTye,
        decimal Price,
        List<List<string>> CalculationItens);
}