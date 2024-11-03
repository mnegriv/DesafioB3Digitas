using CurrencyIngestion.Domain;

namespace CurrencyIngestion.Data.Dto
{
    public record ExchangeOperationDto(
        string currency,
        decimal amount,
        decimal price,
        string type)
    {
        public static explicit operator ExchangeOperationDto(ExchangeOperation op)
        {
            return new ExchangeOperationDto($"{op.Currency}", op.Amount, op.Price, $"{op.Type}");
        }
    }
}