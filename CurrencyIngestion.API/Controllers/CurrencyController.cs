using CurrencyIngestion.API.Handlers;
using CurrencyIngestion.API.Payload;
using CurrencyIngestion.Common.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyIngestion.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyController : Controller
    {
        private readonly IMediator mediator;

        public CurrencyController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("btc/asks")]
        public async Task<ActionResult<Result>> AsksBtc([FromBody] Request request)
        {
            Result result = await mediator.Send(new AskSimulationCommand(request, CurrencyPair.BTCUSD));

            if (request is null)
                return NoContent();

            return Ok(result);
        }

        [HttpPost("btc/bids")]
        public async Task<ActionResult<Result>> BidsBtc([FromBody] Request request)
        {
            Result result = await mediator.Send(new BidSimulationCommand(request, CurrencyPair.BTCUSD));

            if (request is null)
                return NoContent();

            return Ok(result);
        }

        [HttpPost("eth/asks")]
        public async Task<ActionResult<Result>> AsksEth([FromBody] Request request)
        {
            Result result = await mediator.Send(new AskSimulationCommand(request, CurrencyPair.ETHUSD));

            if (request is null)
                return NoContent();

            return Ok(result);
        }

        [HttpPost("eth/bids")]
        public async Task<ActionResult<Result>> BidsEth([FromBody] Request request)
        {
            Result result = await mediator.Send(new BidSimulationCommand(request, CurrencyPair.ETHUSD));

            if (request is null)
                return NoContent();

            return Ok(result);
        }
    }
}