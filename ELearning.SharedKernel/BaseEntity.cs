// <copyright file="BaseEntity.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.SharedKernel;

using ELearning.SharedKernel.Abstractions;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }

    private DateTime createdAtUTC;

    public DateTime CreatedAt()
    {
        return this.createdAtUTC.ToLocalTime();
    }

    protected void CreatedAt(DateTime value)
    {
        this.createdAtUTC = value.ToUniversalTime();
    }

    private DateTime? updatedAtUTC;

    public DateTime? UpdatedAt()
    {
        return this.updatedAtUTC?.ToLocalTime();
    }

    protected void UpdatedAt(DateTime? value)
    {
        this.updatedAtUTC = value?.ToUniversalTime();
    }

    private List<IDomainEvent> domainEvents = new List<IDomainEvent>();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => this.domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        this.domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        this.domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        this.domainEvents.Clear();
    }

    protected BaseEntity()
    {
        this.Id = Guid.NewGuid();
        this.CreatedAt(DateTime.UtcNow);
    }
}