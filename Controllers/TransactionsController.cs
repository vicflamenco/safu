using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using safuCHARTS.Models;
using safuCHARTS.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace safuCHARTS.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly Random _random;

        private readonly string[] _apiKeys =
        {
            "ckey_1af398529c19459493fe71b5e31",
            "ckey_d006bf249a6748538248a94bbca",
            "ckey_3dd5a6f4f05c409c8b20bf98e4f",
            "ckey_e45d2ad98bd24d09a8a99305fed",
            "ckey_224cdaaa12d64af5a4e9e9b3f64"
        };

        public TransactionsController()
        {
            _random = new Random();
        }

        private string GetRandomApiKey()
        {
            return _apiKeys[_random.Next(0, _apiKeys.Length)];
        }

        [HttpGet]
        public async Task<ActionResult> Linear(string tokenAddress, string currentPairAddress, int currentTokenDecimals, DateTime? fromTime, DateTime? toTime)
        {
            var (data, error) = await GetHistoricalData(tokenAddress, currentPairAddress, currentTokenDecimals, fromTime, toTime, 1000);

            if (error.HasValue)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new ErrorResponseMessage(error.Value));
            }

            return Ok(data);
        }

        [HttpGet]
        [Route("bars")]
        public async Task<ActionResult> Bars(string tokenAddress, string currentPairAddress, int currentTokenDecimals, DateTime? fromTime, DateTime? toTime, int resolution = 1)
        {
            var result = new List<Bar>();
            var pageSize = resolution < 15 ? 1000 : resolution < 30 ? 1500 : 2500;

            var (data, error) = await GetHistoricalData(tokenAddress, currentPairAddress, currentTokenDecimals, fromTime, toTime, pageSize);

            if (error.HasValue)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new ErrorResponseMessage(error.Value));
            }

            if (!data.Any())
            {
                return Ok(result);
            }

            data = data.OrderBy(d => d.Time).ToList();
            var initialTime = data.FirstOrDefault().Time;
            var finalTime = data.LastOrDefault().Time;
            var openTime = initialTime;

            do
            {
                var closeTime = openTime + 60 * resolution;
                var transactions = data.Where(t => t.Time >= openTime && t.Time < closeTime).ToList();

                if (transactions.Any())
                {
                    var first = transactions.FirstOrDefault();

                    var open = first.Value;
                    var close = transactions.LastOrDefault().Value;
                    var high = transactions.Max(t => t.Value);
                    var low = transactions.Min(t => t.Value);

                    result.Add(new Bar
                    {
                        Time = first.Time,
                        Open = open,
                        High = high,
                        Low = low,
                        Close = close,
                        Volume = 0
                    });
                }

                openTime = closeTime;
            }
            while (openTime < finalTime);

            return Ok(result);
        }

        private async Task<(List<HistoricDataItem> Data, CovalentResult? Error)> GetHistoricalData(string tokenAddress, string currentPairAddress, int currentTokenDecimals, DateTime? fromTime, DateTime? toTime, int pageSize)
        {
            var historicData = new List<HistoricDataItem>();

            var tokenAddresses = new string[]
            {
                "0x831753dd7087cac61ab5644b308642cc1c33dc13",
                "0x0000000000000000000000000000000000001010",
                "0x7ceb23fd6bc0add59e62ac25578270cff1b9f619",
                "0xf28164a485b0b2c90639e47b0f377b4a438a16b1"
            };

            if (tokenAddresses.Contains(tokenAddress))
            {
                return (historicData, CovalentResult.TokenAddressInvalid);
            }

            var quickSwapRouterAddress = "0xa5e0829caced8ffdd4de3c43696c57f7d7a678ff";
            var key = GetRandomApiKey();
            var queryURL = $"https://api.covalenthq.com/v1/137/address/{currentPairAddress}/transactions_v2/?no-logs=false&page-number=0&page-size={pageSize}&key={key}";

            var handler = new HttpClientHandler
            {
                DefaultProxyCredentials = CredentialCache.DefaultCredentials
            };

            var httpClient = new HttpClient(handler);
            var response = await httpClient.GetAsync(queryURL);
            var remainingAttempts = 4;

            while (response.StatusCode != HttpStatusCode.OK && remainingAttempts > 0)
            {
                Thread.Sleep(3000);
                response = await httpClient.GetAsync(queryURL);
                remainingAttempts--;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return (historicData, CovalentResult.EmptyCovalentResponse);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonSerializer.Deserialize<TransactionQueryResponse>(responseContent);
            var historicalTxData = jsonResponse.Data.Items;

            foreach (var item in historicalTxData)
            {
                var txDateTime = DateTime.Parse(item.BlockSignedAt);
                var unixTimestamp = ((DateTimeOffset)txDateTime).ToUnixTimeSeconds();

                if (fromTime.HasValue && txDateTime < fromTime.Value)
                {
                    continue;
                }

                if (toTime.HasValue && txDateTime > toTime.Value)
                {
                    continue;
                }

                var transactionType = string.Empty;
                var maticUsdPrice = item.GasQuoteRate;

                var logEventsDecoded = item.LogEvents.Where(el => el.Decoded != null && el.Decoded.Name == "Swap").ToList();

                if (logEventsDecoded.Count > 1)
                {
                    continue;
                }

                var decodedSwapEvent = logEventsDecoded.FirstOrDefault();

                if (decodedSwapEvent == null)
                {
                    continue;
                }

                double tokenAmount = 0;
                double maticAmount = 0;
                var toAddress = decodedSwapEvent.Decoded.Params.FirstOrDefault(el => el.Name == "to");

                if (toAddress.Value == quickSwapRouterAddress)
                {
                    // This was a sale
                    transactionType = "sale";

                    var matic = decodedSwapEvent.Decoded.Params.FirstOrDefault(el => el.Name == "amount0Out");
                    var tokens = decodedSwapEvent.Decoded.Params.FirstOrDefault(el => el.Name == "amount1In");

                    maticAmount = double.Parse(matic.Value);
                    tokenAmount = double.Parse(tokens.Value);
                }
                else
                {
                    //This was a buy
                    transactionType = "buy";

                    var tokens = decodedSwapEvent.Decoded.Params.FirstOrDefault(el => el.Name == "amount1Out");
                    var matic = decodedSwapEvent.Decoded.Params.FirstOrDefault(el => el.Name == "amount0In");

                    tokenAmount = double.Parse(tokens.Value);
                    maticAmount = double.Parse(matic.Value);
                }

                if (tokenAmount == 0 || maticAmount == 0)
                {
                    continue;
                }

                if (maticUsdPrice != 0)
                {
                    var priceAtTransactionPoint = ((maticAmount / 1000000000000000000) * maticUsdPrice) / (tokenAmount / (Math.Pow(10, currentTokenDecimals)));

                    if (historicData.Count > 2)
                    {
                        if (unixTimestamp > historicData[^1].Time)
                        {
                            continue;
                        }
                    }

                    historicData.Add(new HistoricDataItem
                    {
                        Time = unixTimestamp,
                        Value = priceAtTransactionPoint
                    });
                }
            }

            return (historicData, null);
        }
    }
}