using CurrencyIngestion.Domain;

namespace CurrencyIngestion.Data.Dto
{
    public record ExchangeSimulationDto(
        string id,
        string currency,
        string type,
        List<ExchangeOperationDto> operations,
        decimal totalAmount,
        decimal totalPrice)
    {
        public static explicit operator ExchangeSimulationDto(ExchangeSimulation simulation)
        {
            return new ExchangeSimulationDto(
                simulation.Identification.ToString(),
                simulation.CurrencyStr,
                simulation.TypeStr,
                simulation.Operations.Select(o => (ExchangeOperationDto)o).ToList(),
                simulation.TotalAmount,
                simulation.TotalPrice);
        }
    }
}