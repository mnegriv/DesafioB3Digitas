﻿using System.Text;

namespace CurrencyIngestion.Model
{
    public record CurrencySummary(
        string Currency,
        decimal HighestPrice, 
        decimal LowestPrice,
        decimal AveragePriceCurrent,
        decimal AveragePriceWithPrevious,
        decimal AveragePriceCumulative)
    {
        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine(Currency);
            sb.AppendLine($"Time: {DateTime.Now.ToLocalTime().ToShortTimeString()}");
            sb.AppendLine($"Highest Price: {HighestPrice}");
            sb.AppendLine($"Lowest Price: {LowestPrice}");
            sb.AppendLine($"Current Average Price: {Math.Round(AveragePriceCurrent, 2)}");
            sb.AppendLine($"Last update Average Price: {Math.Round(AveragePriceWithPrevious, 2)}");
            sb.AppendLine($"Cumulative Average Price: {Math.Round(AveragePriceCumulative, 2)}");

            return sb.ToString();
        }
    };
}