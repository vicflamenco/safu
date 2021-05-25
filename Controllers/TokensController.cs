using Microsoft.AspNetCore.Mvc;
using safuCHARTS.Models;
using safuCHARTS.Services;

namespace safuCHARTS.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TokensController : ControllerBase
    {
        private readonly ICacheService _cacheService;

        public TokensController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        [HttpPost]
        public ActionResult Add([FromBody]TokenHitModel model)
        {
            _cacheService.AddHit(model.TokenAddress);
            _cacheService.SetTokenName(model.TokenAddress, model.TokenName);

            return Ok();
        }

        [HttpGet]
        [Route("top")]
        public ActionResult Top()
        {
            var top10 = _cacheService.GetTop10();
            return Ok(top10);
        }
    }
}