using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Skrawl.API.Services;

namespace Skrawl.API.Infrastructure
{
    public class JwtRefreshTokenCache : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly ILogger<JwtRefreshTokenCache> _logger;
        private readonly IRefreshTokenService _refreshTokenService;

        public JwtRefreshTokenCache(ILogger<JwtRefreshTokenCache> logger, IRefreshTokenService refreshTokenService)
        {
            _logger = logger;
            _refreshTokenService = refreshTokenService;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Refresh token cache cleanup service is starting");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private async void DoWork(object state) {
            await _refreshTokenService.RemoveExpiredRefreshTokensAsync(DateTime.Now);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _logger.LogInformation("Refresh token cache cleanup service finished.");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}