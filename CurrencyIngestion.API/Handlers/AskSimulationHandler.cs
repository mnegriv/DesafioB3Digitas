using CurrencyIngestion.API.Payload;
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

        public AskSimulationHandler(IExchangeSimulationService exchangeSimulationService, ICurrencyRepository currencyRepository)
        {
            this.exchangeSimulationService = exchangeSimulationService;
            this.currencyRepository = currencyRepository;
        }

        public async Task<Result> Handle(AskSimulationCommand request, CancellationToken cancellationToken)
        {
            string latestOrderBookJson = await currencyRepository.GetLatest();

            var askRequest = request.Request;

            var latestOrderBook = OrderBook.FromJson(latestOrderBookJson);

            if (latestOrderBook is null)
                return null;

            var askOperations = latestOrderBook.ToAskOperations().OrderBy(o => o.Price).ToList();

            ExchangeSimulationModel simulationModel = exchangeSimulationService.SimulateOperation(
                "BTC",
                askRequest.Amount,
                askOperations);

            Result result = new(
                Guid.NewGuid(),
                simulationModel.TotalAmount,
                OperationTye.Ask,
                simulationModel.TotalPrice,
                simulationModel.Operations.Select(o => new List<string> { $"{o.Price}", $"{o.Amount}" }).ToList());

            return result;
        }
    }
}