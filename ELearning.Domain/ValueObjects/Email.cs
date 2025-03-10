using ELearning.SharedKernel;
using System.Text.RegularExpressions;

namespace ELearning.Domain.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; private set; }

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
        var regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
        return regex.IsMatch(email);
    }

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value.ToLower();
    }
}