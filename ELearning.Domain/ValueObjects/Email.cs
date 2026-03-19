// <copyright file="Email.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Domain.ValueObjects;

using System.Text.RegularExpressions;
using ELearning.SharedKernel;

public class Email : ValueObject
{
    private static readonly Regex EmailAddressRegex = new(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", RegexOptions.Compiled);

    public string Value { get; init; } = string.Empty;

    private Email()
    {
    }

    private Email(string value)
    {
        this.Value = value;
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty", nameof(email));
        }

        email = email.Trim();

        if (email.Length > 254)
        {
            throw new ArgumentException("Email is too long", nameof(email));
        }

        if (!IsValidEmail(email))
        {
            throw new ArgumentException("Email is invalid", nameof(email));
        }

        return new Email(email);
    }

    private static bool IsValidEmail(string email)
    {
        return EmailAddressRegex.IsMatch(email);
    }

    public override string ToString() => this.Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        // Ensuring case-insensitivity using string.Equals
        yield return this.Value.ToLowerInvariant();
    }
}
