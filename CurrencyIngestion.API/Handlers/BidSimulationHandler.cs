using CurrencyIngestion.API.Payload;
using CurrencyIngestion.Common.Enums;
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
        private readonly IExchangeSimulationRepository exchangeSimulationRepository;

        public BidSimulationHandler(
            IExchangeSimulationService exchangeSimulationService,
            ICurrencyRepository currencyRepository,
            IExchangeSimulationRepository exchangeSimulationRepository)
        {
            this.exchangeSimulationService = exchangeSimulationService;
            this.currencyRepository = currencyRepository;
            this.exchangeSimulationRepository = exchangeSimulationRepository;
        }

        public async Task<Result> Handle(BidSimulationCommand request, CancellationToken cancellationToken)
        {
            string latestOrderBookJson = await currencyRepository.GetLatest(request.Currency);

            var bidRequest = request.Request;

            var latestOrderBook = OrderBook.FromJson(latestOrderBookJson);

            if (latestOrderBook is null)
                return null;

            var bidOperations = latestOrderBook.ToBidOperations(request.Currency).ToList();

            ExchangeSimulationModel simulationModel = exchangeSimulationService.SimulateBidOperation(
                $"{request.Currency}",
                bidRequest.Amount,
                bidOperations);

            Result result = new(
                Guid.NewGuid(),
                simulationModel.TotalAmount,
                OperationType.Bid,
                simulationModel.TotalPrice,
                simulationModel.Operations.Select(o => new List<string> { $"{o.Price}", $"{o.Amount}" }).ToList());

            await exchangeSimulationRepository.Save(simulationModel);

            return result;
        }
    }
}