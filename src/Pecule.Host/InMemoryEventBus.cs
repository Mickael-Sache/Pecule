using Pecule.SharedKernel;

namespace Pecule.Host;

internal sealed class InMemoryEventBus(IServiceProvider services) : IEventBus
{
    public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken)
    {
        foreach (var subscriber in services.GetServices<IEventSubscriber<TMessage>>())
        {
            await subscriber.HandleAsync(message, cancellationToken);
        }
    }
}
