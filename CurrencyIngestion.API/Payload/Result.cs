namespace CurrencyIngestion.API.Payload
{
    public enum OperationTye
    {
        Ask = 1,
        Bid = 2
    }

    public record Result(
        Guid ExchangeIdentification,
        decimal Amount,
        OperationTye OperationTye,
        decimal Price,
        List<List<string>> CalculationItens);
}