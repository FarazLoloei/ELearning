using System.Linq.Expressions;
using FluentAssertions;
using FluentValidation;

namespace ELearning.Application.Tests.Helpers;

internal static class DtoValidationTestHelper
{
    public static void AssertValid<T>(IValidator<T> validator, T instance)
    {
        var result = validator.Validate(instance);
        result.IsValid.Should().BeTrue();
    }

    public static void AssertInvalidFor<T, TProperty>(
        IValidator<T> validator,
        T instance,
        Expression<Func<T, TProperty>> property)
    {
        var result = validator.Validate(instance);
        var propertyName = GetPropertyName(property);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == propertyName);
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

