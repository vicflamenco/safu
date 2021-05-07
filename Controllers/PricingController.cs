using Microsoft.AspNetCore.Mvc;
using safuCHARTS.Models;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace safuCHARTS.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PricingController : ControllerBase
    {
        [HttpGet]
        [Route("matic-usd")]
        public async Task<ActionResult> MaticToUsd()
        {
            var tickers = "MATIC";
            var key = "ckey_224cdaaa12d64af5a4e9e9b3f64";

            var queryURL = $"https://api.covalenthq.com/v1/pricing/tickers/?tickers={tickers}&key={key}";

            var handler = new HttpClientHandler
            {
                DefaultProxyCredentials = CredentialCache.DefaultCredentials
            };

            var httpClient = new HttpClient(handler);
            var response = await httpClient.GetAsync(queryURL);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return Ok(new PricingResponse());
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonSerializer.Deserialize<PricingQueryResponse>(responseContent);

            var price = jsonResponse.Data?.Items.FirstOrDefault()?.QuoteRate;

            var result = new PricingResponse
            {
                QuoteRate = price
            };

            return Ok(result);
        }
    }
}