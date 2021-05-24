using Microsoft.AspNetCore.Mvc;
using safuCHARTS.Services;
using System.Threading.Tasks;

namespace safuCHARTS.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RedisController : ControllerBase
    {
        private readonly IRedisService _redisService;

        public RedisController(IRedisService redisService)
        {
            _redisService = redisService;
        }

        [HttpDelete]
        public async Task<ActionResult> Clear()
        {
            await _redisService.Clear();
            return Ok();
        }
    }
}