using Microsoft.AspNetCore.Mvc;
using safuCHARTS.Services;

namespace safuCHARTS.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CacheController : ControllerBase
    {
        private readonly ICacheService _cacheService;

        public CacheController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        [HttpDelete]
        public ActionResult Clear()
        {
            _cacheService.Clear();
            return Ok();
        }
    }
}