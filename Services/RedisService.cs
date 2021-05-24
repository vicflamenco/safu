using safuCHARTS.Models;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace safuCHARTS.Services
{
    public class RedisService : IRedisService
    {
        private readonly string _tokensHashName = "tokensHash";
        private readonly string _tokenNames = "tokenNames";
        private readonly IDatabase _redisDb;

        public RedisService(IConnectionMultiplexer connectionMultiplexer)
        {
            _redisDb = connectionMultiplexer.GetDatabase(-1);
        }

        public async Task SetTokenName(string tokenAddress, string tokenName)
        {
            await _redisDb.HashSetAsync(_tokenNames, tokenAddress, tokenName);
        }

        public async Task IncrementTokenRequestsCount(string tokenAddress)
        {
            await _redisDb.HashIncrementAsync(_tokensHashName, tokenAddress);
        }

        public async Task<List<Token>> GetTokenNames()
        {
            var tokens = await _redisDb.HashGetAllAsync(_tokenNames);

            return tokens
                .Select(r => new Token
                {
                    TokenAddress = r.Name,
                    TokenName = r.Value
                })
                .ToList();
        }

        public async Task<List<Token>> GetTokensRequestsCount()
        {
            var result = await _redisDb.HashGetAllAsync(_tokensHashName);
            
            var top10 = result
                .Select(r =>
                {
                    return new Token
                    {
                        TokenAddress = r.Name,
                        RequestsCount = int.Parse(r.Value)
                    };
                })
                .OrderByDescending(r => r.RequestsCount)
                .Take(10)
                .ToList();

            var counter = 1;
            top10.ForEach(i => i.Index = counter++);

            return top10;
        }

        public async Task Clear()
        {
            await _redisDb.KeyDeleteAsync(_tokenNames);
            await _redisDb.KeyDeleteAsync(_tokensHashName);
        }
    }
}