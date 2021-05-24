using safuCHARTS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace safuCHARTS.Services
{
    public interface IRedisService
    {
        Task SetTokenName(string tokenAddress, string tokenName);
        Task IncrementTokenRequestsCount(string tokenAddress);
        Task<List<Token>> GetTokenNames();
        Task<List<Token>> GetTokensRequestsCount();
        Task Clear();
    }
}