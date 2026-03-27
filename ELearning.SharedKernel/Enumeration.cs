// <copyright file="Enumeration.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.SharedKernel;

using System.Reflection;

public abstract class Enumeration : IComparable
{
    public int Id { get; private set; }

    public string Name { get; private set; }

    protected Enumeration(int id, string name)
    {
        this.Id = id;
        this.Name = name;
    }

    public override string ToString() => this.Name;

    public static IEnumerable<T> GetAll<T>()
        where T : Enumeration
    {
        var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
        return fields.Select(f => f.GetValue(null)).Cast<T>();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration otherValue)
        {
            return false;
        }

        var typeMatches = this.GetType().Equals(obj.GetType());
        var valueMatches = this.Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode() => this.Id.GetHashCode();

    public int CompareTo(object? obj)
    {
        if (obj is null)
        {
            return 1;
        }

        if (obj is not Enumeration other)
        {
            throw new ArgumentException($"Object must be of type {this.GetType().Name}", nameof(obj));
        }

        return this.Id.CompareTo(other.Id);
    }
}