using safuCHARTS.Models;
using System.Collections.Generic;

namespace safuCHARTS.Services
{
    public interface ICacheService
    {
        void SetTokenName(string tokenAddress, string tokenName);
        void AddHit(string tokenAddress);
        List<Token> GetTokenNames();
        List<Token> GetTop10();
        void Clear();
    }
}