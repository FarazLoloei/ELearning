namespace ELearning.SharedKernel.Abstractions;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}