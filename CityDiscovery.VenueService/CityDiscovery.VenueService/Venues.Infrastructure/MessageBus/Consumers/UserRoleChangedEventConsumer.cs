using CityDiscovery.Venues.Shared.Common.Events.Identity;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CityDiscovery.Venues.Infrastructure.MessageBus.Consumers;

/// <summary>
/// Kullanıcı rolü değiştiğinde log tutmak için (opsiyonel).
/// Örn: User → Owner olduğunda bir şey yapabiliriz.
/// </summary>
public sealed class UserRoleChangedEventConsumer : IConsumer<UserRoleChangedEvent>
{
    private readonly ILogger<UserRoleChangedEventConsumer> _logger;

    public UserRoleChangedEventConsumer(ILogger<UserRoleChangedEventConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<UserRoleChangedEvent> context)
    {
        var msg = context.Message;

        _logger.LogInformation(
            "User role changed: UserId={UserId}, OldRole={OldRole}, NewRole={NewRole}",
            msg.UserId,
            msg.OldRole,
            msg.NewRole);

        // İleride Owner rolüne geçen kullanıcıya özel işlem yapılabilir
        return Task.CompletedTask;
    }
}