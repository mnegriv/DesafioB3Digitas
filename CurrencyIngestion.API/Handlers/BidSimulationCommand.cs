using CurrencyIngestion.API.Payload;
using MediatR;

namespace CurrencyIngestion.API.Handlers
{
    public class BidSimulationCommand : IRequest<Result>
    {
        public Request Request { get; set; }

        public string Currency { get; set; }

        public BidSimulationCommand(Request request, string currency)
        {
            Request = request;
            Currency = currency;
        }
    }
}