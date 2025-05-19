using System;
using Indiego_Backend.Utilities;

namespace Indiego_Backend.Services;

public class SubscriptionExpiryService(IServiceScopeFactory serviceScopeFactory, ILogger<SubscriptionExpiryService> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly ILogger<SubscriptionExpiryService> _logger = logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromDays(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                var subscriptionList = await subscriptionService.Get();
                foreach (var subscription in subscriptionList)
                {
                    if (DatetimeUtility.FromUnixTimestampString(subscription.Expire) < DateTime.UtcNow)
                    {
                        await subscriptionService.Delete(subscription.Id, userService);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while expiring subscriptions");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
}
