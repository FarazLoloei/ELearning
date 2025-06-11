using ELearning.SharedKernel.Abstractions;

namespace ELearning.SharedKernel;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }

    private DateTime createdAtUTC;

    public DateTime CreatedAt()
    {
        return createdAtUTC.ToLocalTime();
    }

    protected void CreatedAt(DateTime value)
    {
        createdAtUTC = value.ToUniversalTime();
    }

    private DateTime? updatedAtUTC;

    public DateTime? UpdatedAt()
    {
        return updatedAtUTC?.ToLocalTime();
    }

    protected void UpdatedAt(DateTime? value)
    {
        updatedAtUTC = value?.ToUniversalTime();
    }

    private List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt(DateTime.UtcNow);
    }
}