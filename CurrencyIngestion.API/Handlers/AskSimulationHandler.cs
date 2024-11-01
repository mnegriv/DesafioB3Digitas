using CurrencyIngestion.API.Payload;
using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Common.Extension;
using CurrencyIngestion.Data;
using CurrencyIngestion.Model;
using CurrencyIngestion.Service;
using MediatR;

namespace CurrencyIngestion.API.Handlers
{
    public class AskSimulationHandler : IRequestHandler<AskSimulationCommand, Result>
    {
        private readonly IExchangeSimulationService exchangeSimulationService;
        private readonly ICurrencyRepository currencyRepository;
        private readonly IExchangeSimulationRepository exchangeSimulationRepository;

        public AskSimulationHandler(
            IExchangeSimulationService exchangeSimulationService,
            ICurrencyRepository currencyRepository,
            IExchangeSimulationRepository exchangeSimulationRepository)
        {
            this.exchangeSimulationService = exchangeSimulationService;
            this.currencyRepository = currencyRepository;
            this.exchangeSimulationRepository = exchangeSimulationRepository;
        }

        public async Task<Result> Handle(AskSimulationCommand request, CancellationToken cancellationToken)
        {
            string latestOrderBookJson = await currencyRepository.GetLatest(request.Currency);

            var askRequest = request.Request;

            var latestOrderBook = OrderBook.FromJson(latestOrderBookJson);

            if (latestOrderBook is null)
                return null;

            var askOperations = latestOrderBook.ToAskOperations(request.Currency).ToList();

            ExchangeSimulationModel simulationModel = exchangeSimulationService.SimulateAskOperation(
                $"{request.Currency}",
                askRequest.Amount,
                askOperations);

            Result result = new(
                Guid.NewGuid(),
                simulationModel.TotalAmount,
                OperationType.Ask,
                simulationModel.TotalPrice,
                simulationModel.Operations.Select(o => new List<string> { $"{o.Price}", $"{o.Amount}" }).ToList());

            await exchangeSimulationRepository.Save(simulationModel);

            return result;
        }
    }
}