namespace ELearning.SharedKernel.Abstractions;

public interface IDomainEvent
{
    DateTime OccurredOnUTC { get; }
}