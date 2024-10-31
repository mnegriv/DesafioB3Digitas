using CurrencyIngestion.API.Payload;
using MediatR;

namespace CurrencyIngestion.API.Handlers
{
    public class AskSimulationCommand : IRequest<Result>
    {
        public Request Request { get; set; }

        public string Currency { get; set; }

        public AskSimulationCommand(Request request, string currency)
        {
            Request = request;
            Currency = currency;
        }
    }
}