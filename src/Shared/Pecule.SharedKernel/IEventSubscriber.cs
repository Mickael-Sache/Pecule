namespace Pecule.SharedKernel;

public interface IEventSubscriber<in TMessage>
{
    Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
}
