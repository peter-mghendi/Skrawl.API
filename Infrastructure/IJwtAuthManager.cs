using System;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Skrawl.API.Data.Models;

namespace Skrawl.API.Infrastructure
{
   public interface IJwtAuthManager
    {
        Task<JwtAuthResult> GenerateTokensAsync(string email, Claim[] claims, DateTime now);
        Task<JwtAuthResult> RefreshAsync(string refreshToken, string accessToken, DateTime now);
        (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token);
    }
}