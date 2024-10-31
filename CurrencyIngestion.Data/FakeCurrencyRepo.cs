﻿using CurrencyIngestion.Common.Enums;

namespace CurrencyIngestion.Data
{
    public class FakeCurrencyRepo : ICurrencyRepository
    {
        public Task<IEnumerable<string>> GetAll(CurrencyPair currency)
        {
            return Task.FromResult(GetOrderBooks());
        }

        public Task Save(string orderBook, CurrencyPair currency)
        {
            return Task.Run(() => Console.WriteLine("Saved"));
        }

        private static IEnumerable<string> GetOrderBooks()
        {
            yield return "{\"data\":{\"timestamp\":\"1730301433\",\"microtimestamp\":\"1730301433357135\",\"bids\":[[\"2692.6\",\"6.87076801\"],[\"2692.2\",\"2.99973002\"],[\"2692.1\",\"7.44164460\"],[\"2692.0\",\"0.00000000\"],[\"2691.6\",\"0.00000000\"],[\"2690.9\",\"11.14847684\"],[\"2688.1\",\"14.05268174\"],[\"2686.9\",\"37.21675218\"]],\"asks\":[[\"2692.9\",\"1.29971920\"],[\"2693.0\",\"23.01866674\"],[\"2693.1\",\"8.79100000\"],[\"2693.2\",\"7.42612587\"],[\"2693.3\",\"5.57092719\"],[\"2694.7\",\"0.00000000\"],[\"2695.0\",\"0.85261537\"],[\"2697.0\",\"1.62000000\"],[\"2697.2\",\"0.00000000\"],[\"2697.5\",\"38.69136954\"]]},\"channel\":\"diff_order_book_ethusd\",\"event\":\"data\"}";
            yield return "{\"data\":{\"timestamp\":\"1730301433\",\"microtimestamp\":\"1730301433379294\",\"bids\":[[\"71943\",\"0.16679786\"],[\"71941\",\"0.06959150\"],[\"71940\",\"0.04100705\"],[\"71939\",\"0.27453800\"],[\"71937\",\"0.00000000\"],[\"71935\",\"0.50458195\"],[\"71921\",\"0.78259955\"],[\"71920\",\"0.50458195\"],[\"71918\",\"0.00000000\"],[\"71912\",\"0.36122503\"],[\"71910\",\"0.00000000\"],[\"71637\",\"0.00142108\"],[\"71618\",\"0.00000000\"]],\"asks\":[[\"71946\",\"0.27800868\"],[\"71951\",\"0.20678246\"],[\"71975\",\"0.00000000\"],[\"71991\",\"1.38908179\"],[\"72024\",\"0.06044416\"],[\"72065\",\"0.50458195\"],[\"72110\",\"1.38677209\"],[\"72115\",\"0.00000000\"]]},\"channel\":\"diff_order_book_btcusd\",\"event\":\"data\"}";
            yield return "{\"data\":{\"timestamp\":\"1730301433\",\"microtimestamp\":\"1730301433668353\",\"bids\":[[\"2692.6\",\"5.57092719\"],[\"2688.1\",\"1.62000000\"],[\"2688.0\",\"28.52098532\"],[\"2678.1\",\"0.03705725\"],[\"2677.8\",\"0.00000000\"]],\"asks\":[[\"2692.9\",\"0.00000000\"],[\"2693.0\",\"15.59200000\"],[\"2693.2\",\"0.00000000\"],[\"2694.2\",\"7.42355413\"]]},\"channel\":\"diff_order_book_ethusd\",\"event\":\"data\"}";
            yield return "{\"data\":{\"timestamp\":\"1730301433\",\"microtimestamp\":\"1730301433707337\",\"bids\":[[\"71944\",\"0.22239505\"],[\"71943\",\"0.00000000\"],[\"71941\",\"0.00000000\"],[\"71940\",\"0.00000000\"],[\"71936\",\"0.32897495\"],[\"71934\",\"0.00000000\"],[\"71933\",\"0.85203636\"],[\"71923\",\"0.00000000\"],[\"71911\",\"0.00000000\"],[\"71833\",\"2.01934083\"],[\"71647\",\"0.00000000\"]],\"asks\":[[\"71950\",\"0.06043500\"],[\"71951\",\"0.00000000\"],[\"71953\",\"0.16677624\"],[\"71955\",\"0.04000000\"],[\"71958\",\"0.50000000\"],[\"71964\",\"0.00000000\"],[\"71967\",\"0.27453800\"],[\"71976\",\"0.19871918\"]]},\"channel\":\"diff_order_book_btcusd\",\"event\":\"data\"}";
        }

        public Task<string> GetLatest(CurrencyPair currency)
        {
            return Task.FromResult("{\"data\":{\"timestamp\":\"1730301433\",\"microtimestamp\":\"1730301433357135\",\"bids\":[[\"2692.6\",\"6.87076801\"],[\"2692.2\",\"2.99973002\"],[\"2692.1\",\"7.44164460\"],[\"2692.0\",\"0.00000000\"],[\"2691.6\",\"0.00000000\"],[\"2690.9\",\"11.14847684\"],[\"2688.1\",\"14.05268174\"],[\"2686.9\",\"37.21675218\"]],\"asks\":[[\"2692.9\",\"1.29971920\"],[\"2693.0\",\"23.01866674\"],[\"2693.1\",\"8.79100000\"],[\"2693.2\",\"7.42612587\"],[\"2693.3\",\"5.57092719\"],[\"2694.7\",\"0.00000000\"],[\"2695.0\",\"0.85261537\"],[\"2697.0\",\"1.62000000\"],[\"2697.2\",\"0.00000000\"],[\"2697.5\",\"38.69136954\"]]},\"channel\":\"diff_order_book_ethusd\",\"event\":\"data\"}");
        }
    }
}