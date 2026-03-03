namespace ELearning.Infrastructure.Outbox;

public interface IOutboxDispatcher
{
    Task<int> DispatchPendingAsync(CancellationToken cancellationToken);
}
