using ELearning.Application.Common.Interfaces;
using MediatR;

namespace ELearning.Application.Common.Behaviors;

public class TransactionBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var isCommandRequest =
            typeof(TRequest).Name.EndsWith("Command", StringComparison.Ordinal) ||
            typeof(TRequest).Namespace?.Contains(".Commands.", StringComparison.Ordinal) == true;

        if (!isCommandRequest)
        {
            return await next();
        }

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var response = await next();
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);
            return response;
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
