using CurrencyIngestion.API.Handlers;
using CurrencyIngestion.API.Payload;
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
            Result result = await mediator.Send(new AskSimulationCommand(request, "BTC"));

            if (request is null)
                return NoContent();

            return Ok(result);
        }

        [HttpPost("btc/bids")]
        public async Task<ActionResult<Result>> BidsBtc([FromBody] Request request)
        {
            Result result = await mediator.Send(new BidSimulationCommand(request, "BTC"));

            if (request is null)
                return NoContent();

            return Ok(result);
        }
    }
}