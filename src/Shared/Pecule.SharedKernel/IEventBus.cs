namespace Pecule.SharedKernel;

public interface IEventBus
{
    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default);

}
