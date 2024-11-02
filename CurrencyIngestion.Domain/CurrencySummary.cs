using System.Text;

namespace CurrencyIngestion.Domain
{
    public record CurrencySummary(
        string Currency,
        decimal HighestPrice,
        decimal LowestPrice,
        decimal AveragePriceCurrent,
        decimal AveragePriceWithPrevious,
        decimal AveragePriceCumulative,
        DateTime UpdateTime)
    {
        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine(Currency);
            sb.AppendLine($"Time: {UpdateTime.ToLocalTime().ToString("dd/MM HH:mm:ss")}");
            sb.AppendLine($"Highest Price: {HighestPrice}");
            sb.AppendLine($"Lowest Price: {LowestPrice}");
            sb.AppendLine($"Current Average Price: {Math.Round(AveragePriceCurrent, 2)}");
            sb.AppendLine($"Last update Average Price: {Math.Round(AveragePriceWithPrevious, 2)}");
            sb.AppendLine($"Cumulative Average Price: {Math.Round(AveragePriceCumulative, 2)}");

            return sb.ToString();
        }
    };
}