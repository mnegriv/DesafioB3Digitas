using CurrencyIngestion.API.Payload;
using CurrencyIngestion.Common.Extension;
using CurrencyIngestion.Data;
using CurrencyIngestion.Model;
using CurrencyIngestion.Service;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyIngestion.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyController : Controller
    {
        private readonly IExchangeSimulationService exchangeSimulationService;
        private readonly ICurrencyRepository currencyRepository;

        public CurrencyController(
            IExchangeSimulationService exchangeSimulationService, ICurrencyRepository currencyRepository)
        {
            this.exchangeSimulationService = exchangeSimulationService;
            this.currencyRepository = currencyRepository;
        }

        [HttpPost("btc/asks")]
        public async Task<ActionResult<Result>> AsksBtc([FromBody] Request request)
        {
            string latestOrderBookJson = await currencyRepository.GetLatest();

            var latestOrderBook = OrderBook.FromJson(latestOrderBookJson);

            if (latestOrderBook is null)
                return NoContent();

            var askOperations = latestOrderBook.ToAskOperations().ToList();

            ExchangeSimulationModel simulationModel = exchangeSimulationService.SimulateOperation(
                "BTC",
                request.Amount,
                askOperations);

            Result result = new(
                Guid.NewGuid(),
                simulationModel.TotalAmount,
                OperationTye.Ask,
                simulationModel.TotalPrice,
                simulationModel.Operations.Select(o => new List<string> { $"{o.Price}", $"{o.Amount}" }).ToList());

            return Ok(result);
        }
    }
}