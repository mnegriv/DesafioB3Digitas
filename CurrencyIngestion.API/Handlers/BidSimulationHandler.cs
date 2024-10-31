using CurrencyIngestion.API.Payload;
using CurrencyIngestion.Common.Extension;
using CurrencyIngestion.Data;
using CurrencyIngestion.Model;
using CurrencyIngestion.Service;
using MediatR;

namespace CurrencyIngestion.API.Handlers
{
    public class BidSimulationHandler : IRequestHandler<BidSimulationCommand, Result>
    {
        private readonly IExchangeSimulationService exchangeSimulationService;
        private readonly ICurrencyRepository currencyRepository;

        public BidSimulationHandler(
            IExchangeSimulationService exchangeSimulationService, ICurrencyRepository currencyRepository)
        {
            this.exchangeSimulationService = exchangeSimulationService;
            this.currencyRepository = currencyRepository;
        }

        public async Task<Result> Handle(BidSimulationCommand request, CancellationToken cancellationToken)
        {
            string latestOrderBookJson = await currencyRepository.GetLatest(request.Currency);

            var bidRequest = request.Request;

            var latestOrderBook = OrderBook.FromJson(latestOrderBookJson);

            if (latestOrderBook is null)
                return null;

            var bidOperations = latestOrderBook.ToBidOperations().OrderByDescending(o => o.Price).ToList();

            ExchangeSimulationModel simulationModel = exchangeSimulationService.SimulateOperation(
                "BTC",
                bidRequest.Amount,
                bidOperations);

            Result result = new(
                Guid.NewGuid(),
                simulationModel.TotalAmount,
                OperationTye.Bid,
                simulationModel.TotalPrice,
                simulationModel.Operations.Select(o => new List<string> { $"{o.Price}", $"{o.Amount}" }).ToList());

            return result;
        }
    }
}