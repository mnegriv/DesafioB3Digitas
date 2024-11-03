using CurrencyIngestion.API.Payload;
using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Data;
using CurrencyIngestion.Domain;
using CurrencyIngestion.Domain.Extensions;
using CurrencyIngestion.Service;
using MediatR;

namespace CurrencyIngestion.API.Handlers
{
    public class AskSimulationHandler : IRequestHandler<AskSimulationCommand, Result?>
    {
        private readonly IExchangeSimulationService exchangeSimulationService;
        private readonly IOrderBookRepository orderBookRepository;
        private readonly IExchangeSimulationRepository exchangeSimulationRepository;

        public AskSimulationHandler(
            IExchangeSimulationService exchangeSimulationService,
            IOrderBookRepository orderBookRepository,
            IExchangeSimulationRepository exchangeSimulationRepository)
        {
            this.exchangeSimulationService = exchangeSimulationService;
            this.orderBookRepository = orderBookRepository;
            this.exchangeSimulationRepository = exchangeSimulationRepository;
        }

        public async Task<Result?> Handle(AskSimulationCommand request, CancellationToken cancellationToken)
        {
            string latestOrderBookJson = await orderBookRepository.GetLatest(request.Currency);

            var askRequest = request.Request;

            var latestOrderBook = OrderBook.FromJson(latestOrderBookJson);

            if (latestOrderBook is null)
                return null;

            var askOperations = latestOrderBook.ToAskOperations(request.Currency).ToList();

            ExchangeSimulation simulationModel = exchangeSimulationService.SimulateAskOperation(
                request.Currency,
                askRequest.Amount,
                askOperations);

            Result result = new(
                Guid.NewGuid(),
                simulationModel.TotalAmount,
                OperationType.Ask,
                simulationModel.TotalPrice,
                simulationModel.Operations);

            await exchangeSimulationRepository.Save(simulationModel);

            return result;
        }
    }
}