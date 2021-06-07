using Microsoft.Extensions.Caching.Memory;
using safuCHARTS.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace safuCHARTS.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly Random _random;
        private readonly string _tokenNamesPrefix = "token";
        private readonly string _tokenHitsPrefix = "hits";
        
        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _random = new Random();
        }

        private List<string> GetAllKeys()
        {
            var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
            var collection = field.GetValue(_memoryCache) as ICollection;
            var items = new List<string>();

            if (collection != null)
            {
                foreach (var item in collection)
                {
                    var methodInfo = item.GetType().GetProperty("Key");
                    var val = methodInfo.GetValue(item);
                    items.Add(val.ToString());
                }
            }
            return items;
        }

        private void Set(string key, string value)
        {
            var expiryTime = DateTimeOffset.Now.AddDays(1);
            _memoryCache.Set(key, value, expiryTime);
        }

        public void SetTokenName(string tokenAddress, string tokenName)
        {
            var key = $"{_tokenNamesPrefix}_{tokenAddress.ToLower()}";
            Set(key, tokenName);
        }

        public void AddHit(string tokenAddress)
        {
            var key = $"{_tokenHitsPrefix}_{tokenAddress.ToLower()}_{DateTime.UtcNow.ToFileTime()}{_random.Next(100, 1000)}";
            Set(key, "1");
        }

        public List<Token> GetTokenNames()
        {
            var allKeys = GetAllKeys();
            var tokenNameKeys = allKeys.Where(t => t.StartsWith(_tokenNamesPrefix)).ToList();

            return tokenNameKeys
                .Select(t =>
                {
                    var tokenAddress = t.Split("_")[1];
                    var tokenName = string.Empty;

                    if (_memoryCache.TryGetValue(t, out tokenName))
                    {
                        tokenName = tokenName.ToString();
                    }

                    return new Token
                    {
                        TokenAddress = tokenAddress,
                        TokenName = tokenName
                    };
                })
                .ToList();
        }

        public List<Token> GetTop10()
        {
            var allKeys = GetAllKeys();
            var tokenHitsKeys = allKeys.Where(t => t.StartsWith(_tokenHitsPrefix)).ToList();

            var top10 = tokenHitsKeys
                .Select(k => k.ToString().Split("_")[1])
                .GroupBy(k => k)
                .Select(g => new Token
                {
                    TokenAddress = g.Key,
                    RequestsCount = g.Count()
                })
                .OrderByDescending(k => k.RequestsCount)
                .Take(10)
                .ToList();

            var tokenNames = GetTokenNames();

            for (var i = 0; i < top10.Count; i++)
            {
                top10[i].Index = i + 1;
                var tokenName = tokenNames.FirstOrDefault(r => r.TokenAddress == top10[i].TokenAddress);

                if (tokenName != null)
                {
                    top10[i].TokenName = tokenName.TokenName;
                }
            }
            return top10;
        }

        public void Clear()
        {
            var allKeys = GetAllKeys();

            foreach (var key in allKeys)
            {
                _memoryCache.Remove(key);
            }
        }
    }
}