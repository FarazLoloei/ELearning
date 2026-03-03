using MediatR;

namespace ELearning.SharedKernel.Abstractions;

public interface IDomainEvent : INotification
{
    DateTime OccurredOnUTC { get; }
}
