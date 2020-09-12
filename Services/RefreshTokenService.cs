using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Skrawl.API.Data;
using Skrawl.API.Data.Models;

namespace Skrawl.API.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ILogger<RefreshTokenService> _logger;
        private readonly SkrawlContext _context;

        public RefreshTokenService(ILogger<RefreshTokenService> logger, SkrawlContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        public async Task<RefreshToken> GetTokenAsync(string tokenString)
        {
            return await _context.RefreshTokens.SingleAsync(r => r.TokenString == tokenString);
        }

        public async Task<RefreshToken> CreateTokenAsync(string email, int expiry, DateTime now)
        {
            var refreshToken = new RefreshToken
            {
                Email = email,
                TokenString = GenerateRefreshTokenString(),
                ExpireAt = now.AddMinutes(expiry)
            };

            var entry = await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return entry.Entity;
        }

        // optional: clean up expired refresh tokens
        public async Task RemoveExpiredRefreshTokensAsync(DateTime now)
        {
            var expiredTokens = _context.RefreshTokens.Where(x => x.ExpireAt < now);
            _context.RefreshTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
        }

        // can be more specific to ip, user agent, device name, etc.
        public async Task RemoveRefreshTokenByEmailAsync(string email)
        {
            var refreshTokens = _context.RefreshTokens.Where(x => x.Email == email);
            _context.RefreshTokens.RemoveRange(refreshTokens);
            await _context.SaveChangesAsync();
        }

        // TODO Improve this to generate truly unique strings
        private static string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}