using MediatR;

namespace ELearning.Application.Common.Behaviors;

// Transaction behavior for request pipeline
public class TransactionBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    // In a real app, you'd inject a DbContext or UnitOfWork here

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // This is a simplified example.
        // In a real application, you would:
        // 1. Begin a transaction
        // 2. Try to execute the next delegate
        // 3. Commit the transaction if successful
        // 4. Roll back if there's an exception

        return await next();

        // Example with actual transaction:
        /*
        using (var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken))
        {
            try
            {
                var response = await next();
                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return response;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
        */
    }
}