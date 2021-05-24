using Microsoft.AspNetCore.Mvc;
using safuCHARTS.Models;
using safuCHARTS.Services;
using System.Linq;
using System.Threading.Tasks;

namespace safuCHARTS.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TokensController : ControllerBase
    {
        private readonly IRedisService _redisService;

        public TokensController(IRedisService redisService)
        {
            _redisService = redisService;
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody]TokenHitModel model)
        {
            await _redisService.IncrementTokenRequestsCount(model.TokenAddress);
            await _redisService.SetTokenName(model.TokenAddress, model.TokenName);

            return Ok();
        }

        [HttpGet]
        [Route("top")]
        public async Task<ActionResult> Top()
        {
            var top10 = await _redisService.GetTokensRequestsCount();
            var tokens = await _redisService.GetTokenNames();

            for (var i = 0; i < top10.Count; i++)
            {
                var tokenName = tokens.FirstOrDefault(r => r.TokenAddress == top10[i].TokenAddress);

                if (tokenName != null)
                {
                    top10[i].TokenName = tokenName.TokenName;
                }
            }

            return Ok(top10);
        }
    }
}