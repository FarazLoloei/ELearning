// <copyright file="ValueObject.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.SharedKernel;

public abstract class ValueObject
{
    protected static bool EqualOperator(ValueObject? left, ValueObject? right) =>
        left is not null && right is not null && left.Equals(right);

    protected static bool NotEqualOperator(ValueObject? left, ValueObject? right) =>
        !EqualOperator(left, right);

    protected abstract IEnumerable<object> GetEqualityComponents();

    public override sealed bool Equals(object? obj) =>
        obj is ValueObject other && this.GetType() == other.GetType() &&
        this.GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());

    public override sealed int GetHashCode() =>
        this.GetEqualityComponents().Aggregate(0, (hash, obj) =>
            HashCode.Combine(hash, obj?.GetHashCode() ?? 0));
}
