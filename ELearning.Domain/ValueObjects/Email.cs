using ELearning.SharedKernel;
using System.Text.RegularExpressions;

namespace ELearning.Domain.ValueObjects;

public class Email : ValueObject
{
    private static readonly Regex EmailAddressRegex = new(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", RegexOptions.Compiled);

    public string Value { get; init; }

    private Email()
    { }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        email = email.Trim();

        if (email.Length > 254)
            throw new ArgumentException("Email is too long", nameof(email));

        if (!IsValidEmail(email))
            throw new ArgumentException("Email is invalid", nameof(email));

        return new Email(email);
    }

    private static bool IsValidEmail(string email)
    {
        return EmailAddressRegex.IsMatch(email);
    }

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        // Ensuring case-insensitivity using string.Equals
        yield return Value.ToLowerInvariant();
    }
}