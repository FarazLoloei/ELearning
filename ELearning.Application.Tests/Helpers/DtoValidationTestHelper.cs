using System.Linq.Expressions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;

namespace ELearning.Application.Tests.Helpers;

internal static class DtoValidationTestHelper
{
    public static void AssertValid<T>(IValidator<T> validator, T instance)
    {
        var result = validator.Validate(instance);
        result.IsValid.Should().BeTrue($"expected {typeof(T).Name} to be valid, but got: {FormatErrors(result)}");
    }

    public static void AssertInvalidFor<T, TProperty>(
        IValidator<T> validator,
        T instance,
        Expression<Func<T, TProperty>> property)
    {
        var result = validator.Validate(instance);
        var propertyName = GetPropertyName(property);
        result.IsValid.Should().BeFalse($"expected {typeof(T).Name} to be invalid for '{propertyName}'.");
        result.Errors
            .Where(error => error.PropertyName == propertyName)
            .Should()
            .NotBeEmpty($"expected validation error for '{propertyName}', but got: {FormatErrors(result)}");
    }

    private static string FormatErrors(ValidationResult result)
    {
        if (result.Errors.Count == 0)
        {
            return "no validation errors";
        }

        return string.Join("; ", result.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
    }

    private static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> property)
    {
        if (property.Body is MemberExpression memberExpression)
        {
            return memberExpression.Member.Name;
        }

        if (property.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression unaryMember)
        {
            return unaryMember.Member.Name;
        }

        throw new ArgumentException("Expression must target a property.", nameof(property));
    }
}

