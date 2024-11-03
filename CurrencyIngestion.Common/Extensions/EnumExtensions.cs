using CurrencyIngestion.Common.Enums;

namespace CurrencyIngestion.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string ToOrderBookChannel(this CurrencyPair currencyPair) =>
            string.Format(Constants.ORDERBOOK_CHANNEL_IDENTIFIER, currencyPair.ToString().ToLower());
    }
}