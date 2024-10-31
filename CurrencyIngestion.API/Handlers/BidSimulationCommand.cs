using CurrencyIngestion.API.Payload;
using CurrencyIngestion.Common.Enums;
using MediatR;

namespace CurrencyIngestion.API.Handlers
{
    public class BidSimulationCommand : IRequest<Result>
    {
        public Request Request { get; set; }

        public CurrencyPair Currency { get; set; }

        public BidSimulationCommand(Request request, CurrencyPair currency)
        {
            Request = request;
            Currency = currency;
        }
    }
}