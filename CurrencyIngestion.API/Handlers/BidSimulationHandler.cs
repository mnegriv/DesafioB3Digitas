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
    public class BidSimulationHandler : IRequestHandler<BidSimulationCommand, Result?>
    {
        private readonly IExchangeSimulationService exchangeSimulationService;
        private readonly IOrderBookRepository orderBookRepository;
        private readonly IExchangeSimulationRepository exchangeSimulationRepository;

        public BidSimulationHandler(
            IExchangeSimulationService exchangeSimulationService,
            IOrderBookRepository orderBookRepository,
            IExchangeSimulationRepository exchangeSimulationRepository)
        {
            this.exchangeSimulationService = exchangeSimulationService;
            this.orderBookRepository = orderBookRepository;
            this.exchangeSimulationRepository = exchangeSimulationRepository;
        }

        /// <summary>
        /// Handles a 'Bid' simulation request received from the controller
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A Task with the simulation data results</returns>
        public async Task<Result?> Handle(BidSimulationCommand request, CancellationToken cancellationToken)
        {
            var bidRequest = request.Request;

            string channelName = request.Currency.ToOrderBookChannel();

            OrderBook? latestOrderBook = await orderBookRepository.GetLatest(channelName);

            if (latestOrderBook is null)
                return null;

            var bidOperations = latestOrderBook.ToBidOperations(request.Currency).ToList();

            ExchangeSimulation simulationModel = exchangeSimulationService.SimulateBidOperation(
                request.Currency,
                bidRequest.Amount,
                bidOperations);

            Result result = new(
                simulationModel.Identification,
                simulationModel.TotalAmount,
                OperationType.Bid,
                simulationModel.TotalPrice,
                simulationModel.Operations);

            await exchangeSimulationRepository.Save(simulationModel);

            return result;
        }
    }
}