using CurrencyIngestion.API;
using CurrencyIngestion.API.Payload;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;

namespace CurrencyIngestion.Test.CurrencyIngestion.API.Test.Controllers
{
    public class CurrencyControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient client;

        public CurrencyControllerTests(WebApplicationFactory<Program> factory)
        {
            this.client = factory.CreateClient();
        }

        [Theory]
        [InlineData("btc/asks")]
        [InlineData("btc/bids")]
        [InlineData("eth/asks")]
        [InlineData("eth/bids")]
        public async Task Given_ApiEndpoint_When_TryCallEndpoint_Then_SuccessIsObtained(string endpoint)
        {
            Request request = new(Amount: 100);

            var response = await client.PostAsJsonAsync($"Currency/{endpoint}", request);

            response.EnsureSuccessStatusCode();
        }
    }
}