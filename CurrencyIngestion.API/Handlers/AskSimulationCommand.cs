using CurrencyIngestion.API.Payload;
using CurrencyIngestion.Common.Enums;
using MediatR;

namespace CurrencyIngestion.API.Handlers
{
    public class AskSimulationCommand : IRequest<Result>
    {
        public Request Request { get; set; }

        public CurrencyPair Currency { get; set; }

        public AskSimulationCommand(Request request, CurrencyPair currency)
        {
            Request = request;
            Currency = currency;
        }
    }
}