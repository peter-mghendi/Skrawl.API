using System;
using System.Threading.Tasks;
using Skrawl.API.Data.Models;

namespace Skrawl.API.Services
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken> GetTokenAsync(string refreshTokenString);
        Task<RefreshToken> CreateTokenAsync(string email, int expiry, DateTime now);
        Task RemoveExpiredRefreshTokensAsync(DateTime now);
        Task RemoveRefreshTokenByEmailAsync(string email);
    }
}