using FluentValidation;
using MediatR;

namespace ELearning.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse>
    (IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .Select(f => (f.PropertyName, f.ErrorMessage))
                .ToList();

            if (failures.Any())
            {
                throw new ValidationException((IEnumerable<FluentValidation.Results.ValidationFailure>)failures);
            }
        }

        return await next();
    }
}