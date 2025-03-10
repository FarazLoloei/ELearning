namespace ELearning.SharedKernel;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}