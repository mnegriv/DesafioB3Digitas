using CurrencyIngestion.API.Payload;
using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Common.Extensions;
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

        /// <summary>
        /// Handles an 'Ask' simulation request received from the controller
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A Task with the simulation data results</returns>
        public async Task<Result?> Handle(AskSimulationCommand request, CancellationToken cancellationToken)
        {
            var askRequest = request.Request;

            string channelName = request.Currency.ToOrderBookChannel();

            OrderBook? latestOrderBook = await orderBookRepository.GetLatest(channelName);

            if (latestOrderBook is null)
                return null;

            var askOperations = latestOrderBook.ToAskOperations(request.Currency).ToList();

            ExchangeSimulation simulationModel = exchangeSimulationService.SimulateAskOperation(
                request.Currency,
                askRequest.Amount,
                askOperations);

            Result result = new(
                simulationModel.Identification,
                simulationModel.TotalAmount,
                OperationType.Ask,
                simulationModel.TotalPrice,
                simulationModel.Operations);

            await exchangeSimulationRepository.Save(simulationModel);

            return result;
        }
    }
}