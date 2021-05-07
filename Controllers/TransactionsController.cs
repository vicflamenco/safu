using Microsoft.AspNetCore.Mvc;
using safuCHARTS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace safuCHARTS.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TransactionsController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> Index(string tokenAddress, string currentPairAddress, int currentTokenDecimals, DateTime? fromTime, DateTime? toTime)
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
                return Ok(historicData);
            }

            var key = "ckey_224cdaaa12d64af5a4e9e9b3f64";
            var pageSize = 1000;
            var quickSwapRouterAddress = "0xa5e0829caced8ffdd4de3c43696c57f7d7a678ff";
            var queryURL = $"https://api.covalenthq.com/v1/137/address/{currentPairAddress}/transactions_v2/?no-logs=false&page-size={pageSize}&key={key}";

            var handler = new HttpClientHandler
            {
                DefaultProxyCredentials = CredentialCache.DefaultCredentials
            };

            var httpClient = new HttpClient(handler);
            var response = await httpClient.GetAsync(queryURL);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return Ok(historicData);
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

            return Ok(historicData);
        }
    }
}