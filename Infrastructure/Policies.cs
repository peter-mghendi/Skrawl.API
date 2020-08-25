using Microsoft.AspNetCore.Authorization;
using Skrawl.API.Services;

namespace Skrawl.API.Infrastructure
{
    public class Policies
    {
        public const string Admin = nameof(Admin);
        public const string User = nameof(User);
        
        public static AuthorizationPolicy AdminPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(Admin).Build();
        }
        public static AuthorizationPolicy UserPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(User).Build();
        }
    }
}